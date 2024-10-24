﻿using System;
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
using StardewValleyManager.Models;
using StardewValleyManager.Services;
using StardewValleyManager.ViewModels.Factories;
using StardewValleyManager.Views;

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
    private bool _savingOrLoading = false;

    [ObservableProperty]
    private SaveInfoModel? _activeSave = new SaveInfoModel();

    [ObservableProperty]
    private string _gitErrorTitle = "";

    [ObservableProperty]
    private string _gitErrorMessage = "";

    [ObservableProperty]
    private bool _showGitError = false;

    private GitService git;

    private SettingsService settingsService;

    private GameSaveFileService saveService;

    private IWindowFactory<CommitPropertiesWindow> commitPropertiesWindowFactory;

    private string repoName;

    private string saveLocation;

    public SavesViewModel(GitService gitService, SettingsService settingsService, GameSaveFileService saveService, IWindowFactory<CommitPropertiesWindow> commitPropertiesWindowFactory)
    {
        git = gitService;
        this.settingsService = settingsService;
        this.saveService = saveService;

        this.commitPropertiesWindowFactory = commitPropertiesWindowFactory;

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

            if (!File.Exists($"{saveLocation}/{folder}/{folder}") || !File.Exists($"{saveLocation}/{folder}/SaveGameInfo"))
            {
                continue;
            }

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
            info.FarmTypeIconPath = saveService.GetFarmTypeIconPath();
            info.PetIconPath = saveService.GetPetIconPath();

            Saves.Add(info);
        }

    }

    [RelayCommand]
    private async Task LoadMissingSavesFromGitHub()
    {
        SavingOrLoading = true;

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

        SavingOrLoading = false;
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
        SavingOrLoading = true;

        string saveName = Save.Name;

        System.Diagnostics.Debug.WriteLine($"Loading save history for {saveName}");

        try
        {
            IReadOnlyList<GitHubCommit> commitHistory = await git.GetCommitHistory(saveName);

            Save.SaveHistory.Clear();

            SaveHistoryItemModel localSaveItem = await ParseSaveInfo(saveName, null, true);
            Save.SaveHistory.Add(localSaveItem);

            foreach (GitHubCommit commit in commitHistory)
            {
                SaveHistoryItemModel item = await ParseSaveInfo(saveName, commit, false);

                Save.SaveHistory.Add(item);
            }
        }
        catch (ArgumentException e)
        {
            GitErrorTitle = "GitHub Authentication Error";
            GitErrorMessage = "There was an error accessing your GitHub account. Please go to Settings > GitHub > Account Setup > Setup Account to fix the authentication error.";
            ShowGitError = true;

            Save.SaveHistory.Clear();

            SaveHistoryItemModel localSaveItem = await ParseSaveInfo(saveName, null, true);
            Save.SaveHistory.Add(localSaveItem);
        }
        catch (NotFoundException e)
        {
            GitErrorTitle = "Save History Load Error";
            GitErrorMessage = "There was an error loading your save history. Please ensure that your GitHub repository is configured properly in Settings > GitHub > Account Setup > Repository Name.";
            ShowGitError = true;

            Save.SaveHistory.Clear();

            SaveHistoryItemModel localSaveItem = await ParseSaveInfo(saveName, null, true);
            Save.SaveHistory.Add(localSaveItem);
        }
        catch (RateLimitExceededException e)
        {
            GitErrorTitle = "GitHub Authentication Error";
            GitErrorMessage = "There was an error accessing your GitHub account. Please go to Settings > GitHub > Account Setup > Setup Account to fix the authentication error.";
            ShowGitError = true;

            Save.SaveHistory.Clear();

            SaveHistoryItemModel localSaveItem = await ParseSaveInfo(saveName, null, true);
            Save.SaveHistory.Add(localSaveItem);
        }
        finally
        {
            ActiveSave = Save;
            SavingOrLoading = false;
        }
    }

    private async Task<SaveHistoryItemModel> ParseSaveInfo(string saveName, GitHubCommit? commit, bool isLocal)
    {
        SaveHistoryItemModel item = new SaveHistoryItemModel();

        if (isLocal)
        {
            string localSaveLocation = $"{saveLocation}/{saveName}";

            FileInfo fi = new FileInfo(localSaveLocation);
            item.CommitDate = fi.LastWriteTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            item.CommitSha = "";
            item.SaveSource = "Local";

            saveService.LoadSaveGameInfoFile($"{localSaveLocation}/SaveGameInfo");
        } else
        {
            string commitSHA = commit.Sha;
            DateTimeOffset commitDateOffset = commit.Commit.Author.Date;

            item.CommitSha = commitSHA;
            item.CommitDate = commitDateOffset.ToLocalTime().LocalDateTime.ToString("MM/dd/yyyy hh:mm:ss tt");
            item.SaveSource = "GitHub";

            string saveGameInfo = await git.GetCommitContent($"{saveName}/SaveGameInfo", commitSHA);

            saveService.LoadSaveGameInfoXML(saveGameInfo);
        }
        
        item.GameVersion = saveService.GetGameVersion();
        item.SaveName = saveName;

        item.Year = saveService.GetYear();
        item.Season = saveService.GetSeason();
        item.Day = saveService.GetDay();
        item.Money = saveService.GetPlayerMoney();
        item.TotalMoneyEarned = saveService.GetTotalEarnedMoney();

        ObservableCollection<PlayerRelationshipModel> playerRelationships = new ObservableCollection<PlayerRelationshipModel>(saveService.GetPlayerRelationships());
        item.RelationshipStatus = playerRelationships;

        item.FarmingLevel = saveService.GetPlayerFarmingLevel();
        item.MiningLevel = saveService.GetPlayerMiningLevel();
        item.CombatLevel = saveService.GetPlayerCombatLevel();
        item.ForagingLevel = saveService.GetPlayerForagingLevel();
        item.FishingLevel = saveService.GetPlayerFishingLevel();

        ObservableCollection<InventoryItemModel> playerInventory = new ObservableCollection<InventoryItemModel>(saveService.GetPlayerInventory());
        item.PlayerInventory = playerInventory;

        return item;
    }

    [RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task CommitSelectedSaves()
    {
        SavingOrLoading = true;

        try
        {
            IEnumerable<string> selectedSaves = Saves.Where(save => save.IsSelected).Select(save => save.Name);
            await git.CommitAllSaves(selectedSaves.ToArray());

            await LoadSaveHistory(ActiveSave);
        } catch (ArgumentException e)
        {
            ShowGitError = true;
        } catch (NotFoundException e)
        {
            ShowGitError = true;
        } finally
        {
            SavingOrLoading = false;
        }
    }

    [RelayCommand(CanExecute = nameof(CanCommitSave))]
    private async Task CommitSave(string SaveName)
    {
        SavingOrLoading = true;

        try
        {
            await git.CommitSaveFolder(SaveName);

            await LoadSaveHistory(ActiveSave);
        }
        catch (ArgumentException e)
        {
            ShowGitError = true;
        }
        catch (NotFoundException e)
        {
            ShowGitError = true;
        }
        finally
        {
            SavingOrLoading = false;
        }
    }

    [RelayCommand]
    private void OpenCommitProperties(SaveHistoryItemModel saveProperties)
    {
        commitPropertiesWindowFactory.CreateWindow(saveProperties);
    }

    private bool CanCommitSave() => !SavingOrLoading;

}
