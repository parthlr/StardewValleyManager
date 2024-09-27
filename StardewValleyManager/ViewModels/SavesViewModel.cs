using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using Avalonia.Controls;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels;
public partial class SavesViewModel : ViewModelBase
{

    [ObservableProperty]
    private ObservableCollection<SaveInfoModel> _saves = [];

    [ObservableProperty]
    private string _searchQuery = "";

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CommitSelectedSavesCommand))]
    [NotifyCanExecuteChangedFor(nameof(CommitSaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadSaveFromCommitCommand))]
    private bool _saving = false;

    [ObservableProperty]
    private SaveInfoModel? _activeSave = new SaveInfoModel();

    private GitService git;

    private SettingsService settingsService;

    private GameSaveFileService saveService;

    private string repoName;

    private string saveLocation;

    public SavesViewModel(GitService gitService, SettingsService settingsService, GameSaveFileService saveService)
    {
        git = gitService;
        this.settingsService = settingsService;
        this.saveService = saveService;

        repoName = settingsService.GetSettingsValue("repository");
        saveLocation = settingsService.GetSettingsValue("savesLocation");

        LoadSaveLocations();

        //git.Authenticate();
        //git.CheckAndCreateRepository("test-repo2");
    }

    [RelayCommand]
    private void LoadSaveLocations()
    {
        Saves.Clear();

        string?[] saveFolders = Directory.GetDirectories(saveLocation)
                            .Select(Path.GetFileName)
                            .ToArray();

        foreach (string folder in saveFolders)
        {
            System.Diagnostics.Debug.WriteLine(folder);

            FileInfo fi = new FileInfo($"{saveLocation}/{folder}");

            SaveInfoModel info = new SaveInfoModel();
            info.Name = folder;
            info.Date = fi.LastWriteTime.ToShortDateString();
            info.IsEnabled = true;
            info.IsVisible = true;
            info.LoadFarmName();

            saveService.LoadSaveFile($"{saveLocation}/{folder}/{folder}");
            saveService.LoadSaveGameInfoFile($"{saveLocation}/{folder}/SaveGameInfo");

            info.PlayerName = saveService.GetPlayerName();
            info.FarmType = saveService.GetFarmType();

            Saves.Add(info);
        }

    }

    [RelayCommand]
    private async Task LoadMissingSavesFromGitHub()
    {
        IReadOnlyDictionary<string, IReadOnlyList<RepositoryContent>> savesInGitHub = await git.GetSavesFromRepository();

        List<string> savesToDownload = new List<string>();

        // Get saves that need to be downloaded
        foreach (string gitSave in savesInGitHub.Keys)
        {
            bool containsSave = false;
            foreach (SaveInfoModel save in Saves)
            {
                if (gitSave.Equals(save.Name))
                {
                    containsSave = true;
                    break;
                }
            }

            if (!containsSave)
            {
                savesToDownload.Add(gitSave);
            }
        }

        // Download missing saves
        foreach (string save in savesToDownload)
        {
            IReadOnlyList<RepositoryContent> downloadContent = savesInGitHub[save];
            Directory.CreateDirectory($"{saveLocation}/{save}");

            foreach (RepositoryContent file in downloadContent)
            {
                // Download each file to the save
                bool downloadResult = await DownloadFromURL(file.DownloadUrl, $"{saveLocation}/{save}/{file.Name}");
            }
        }

        LoadSaveLocations();
    }

    private async Task<bool> DownloadFromURL(string url, string location)
    {
        using (HttpClient client = new HttpClient())
        {
            using (HttpResponseMessage response = await client.GetAsync(url))
            {
                if (response.IsSuccessStatusCode)
                {
                    byte[] fileContent = await response.Content.ReadAsByteArrayAsync();
                    if (fileContent != null)
                    {
                        try
                        {
                            using (FileStream fs = new FileStream(location, System.IO.FileMode.Create, FileAccess.Write))
                            {
                                fs.Write(fileContent, 0, fileContent.Length);
                            }
                        }
                        catch (Exception ex)
                        {
                            return false;
                        }
                    } else
                    {
                        return false;
                    }
                } else
                {
                    return false;
                }
            }
        }

        return true;
    }

    partial void OnSearchQueryChanged(string value)
    {
        string query = value.Trim();

        if (query.Equals(""))
        {
            foreach (SaveInfoModel save in Saves)
            {
                save.IsVisible = true;
            }
        } else
        {
            foreach (SaveInfoModel save in Saves)
            {
                if (save.Name.ToLower().Contains(SearchQuery))
                {
                    save.IsVisible = true;
                } else
                {
                    save.IsVisible = false;
                }
            }
        }
    }

    [RelayCommand]
    private async Task LoadSaveHistory(SaveInfoModel Save)
    {
        System.Diagnostics.Debug.WriteLine($"Loading save history for {Save.Name}");

        IReadOnlyList<GitHubCommit> commitHistory = await git.GetCommitHistory(Save.Name);

        Save.SaveHistory.Clear();

        foreach (GitHubCommit commit in commitHistory)
        {
            string commitSHA = commit.Sha;
            DateTimeOffset commitDateOffset = commit.Commit.Author.Date;

            SaveHistoryItemModel item = new SaveHistoryItemModel();
            item.CommitSha = commitSHA.Substring(0, 7);
            item.CommitDate = commitDateOffset.ToLocalTime().LocalDateTime.ToString();

            string saveGameInfo = await git.GetCommitContent($"{Save.Name}/SaveGameInfo", commitSHA);

            saveService.LoadSaveGameInfoXML(saveGameInfo);

            item.Year = saveService.GetYear();
            item.Season = saveService.GetSeason();
            item.Day = saveService.GetDay();

            Save.SaveHistory.Add(item);
        }

        ActiveSave = Save;
    }

    [RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task CommitSelectedSaves()
    {
        Saving = true;

        IEnumerable<string> selectedSaves = Saves.Where(save => save.IsSelected).Select(save => save.Name);
        await git.CommitAllSaves(selectedSaves.ToArray());

        Saving = false;
    }

    [RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task CommitSave(string SaveName)
    {
        Saving = true;

        await git.CommitSaveFolder(SaveName);

        Saving = false;
    }

    private bool CanCommitSave() => !Saving;

    [RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task LoadSaveFromCommit(string SaveName)
    {
        Saving = true;

        System.Diagnostics.Debug.WriteLine($"Getting content from commit for save file {SaveName}");

        Commit latestCommit = await git.GetLatestCommit();

        string commitContent = await git.GetCommitContent($"{SaveName}/{SaveName}", latestCommit.Sha);

        Directory.CreateDirectory($"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/StardewValley/Save_Backups");

        string outputFilePath = $"{Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData)}/StardewValley/Save_Backups/{SaveName}";

        if (!string.IsNullOrEmpty(commitContent))
        {
            System.Diagnostics.Debug.WriteLine("Writing to file");
            File.WriteAllText(outputFilePath, commitContent);
        }

        System.Diagnostics.Debug.WriteLine($"Finished saving local copy for {SaveName}");

        Saving = false;
    }

}
