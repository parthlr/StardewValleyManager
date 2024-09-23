using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Configuration;
using System.IO;
using System.Linq;
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

    private string repoName;

    private string saveLocation;

    public SavesViewModel(GitService gitService, SettingsService settingsService)
    {
        git = gitService;
        this.settingsService = settingsService;

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
            SaveInfoModel info = new SaveInfoModel();
            info.Name = folder;
            info.Date = "08/29/2024";
            info.IsEnabled = true;
            info.IsVisible = true;
            info.LoadFarmName();

            info.SaveService = new SaveFileService();
            info.SaveService.LoadSaveFile($"{saveLocation}/{folder}/{folder}");
            info.SaveService.LoadSaveGameInfoFile($"{saveLocation}/{folder}/SaveGameInfo");

            info.PlayerName = info.SaveService.GetPlayerName();
            info.FarmType = info.SaveService.GetFarmType();

            Saves.Add(info);
        }

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
            SaveHistoryItemModel item = new SaveHistoryItemModel();
            item.CommitSha = commit.Sha;
            item.CommitDate = commit.Commit.Author.Date.ToString();

            string saveGameInfo = await git.GetCommitContent($"{Save.Name}/SaveGameInfo", commit.Sha);

            item.SaveService = new SaveFileService();
            item.SaveService.LoadSaveGameInfoXML(saveGameInfo);
            item.Year = item.SaveService.GetYear();
            item.Season = item.SaveService.GetSeason();
            item.Day = item.SaveService.GetDay();

            Save.SaveHistory.Add(item);
        }

        ActiveSave = Save;
    }

    /*[RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task CommitAllSaves()
    {
        Saving = true;

        await git.CommitAllSaves(Saves.ToArray());

        Saving = false;
    }*/

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
