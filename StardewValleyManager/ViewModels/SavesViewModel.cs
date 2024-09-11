﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels;
public partial class SavesViewModel : ObservableObject
{

    [ObservableProperty]
    private ObservableCollection<SaveInfoModel> _saves = [];

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(CommitSelectedSavesCommand))]
    [NotifyCanExecuteChangedFor(nameof(CommitSaveCommand))]
    [NotifyCanExecuteChangedFor(nameof(LoadSaveFromCommitCommand))]
    private bool _saving = false;

    [ObservableProperty]
    private SaveInfoModel? _activeSave = new SaveInfoModel();

    private GitService git;

    public SavesViewModel()
    {
        LoadSaveLocations(Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData) + "/StardewValley/Saves");

        git = GitService.Instance;
        git.Authenticate();
        //git.CheckAndCreateRepository("test-repo2");
    }

    private void LoadSaveLocations(string rootDir)
    {
        string?[] saveFolders = Directory.GetDirectories(rootDir)
                            .Select(Path.GetFileName)
                            .ToArray();

        foreach (string folder in saveFolders)
        {
            System.Diagnostics.Debug.WriteLine(folder);
            SaveInfoModel info = new SaveInfoModel();
            info.Name = folder;
            info.Date = "08/29/2024";
            info.IsEnabled = true;
            info.LoadFarmName();

            info.SaveService = new SaveFileService();
            info.SaveService.LoadSaveFile($"{rootDir}/{folder}/{folder}");
            info.SaveService.LoadSaveGameInfoFile($"{rootDir}/{folder}/SaveGameInfo");

            info.PlayerName = info.SaveService.GetPlayerName();
            info.FarmType = info.SaveService.GetFarmType();

            Saves.Add(info);
        }

    }

    [RelayCommand]
    private async Task LoadSaveHistory(SaveInfoModel Save)
    {
        System.Diagnostics.Debug.WriteLine($"Loading save history for {Save.Name}");

        if (Save.SaveHistory.Count == 0)
        {
            IReadOnlyList<GitHubCommit> commitHistory = await git.GetCommitHistory("test-repo2", Save.Name);

            foreach (GitHubCommit commit in commitHistory)
            {
                SaveHistoryItemModel item = new SaveHistoryItemModel();
                item.CommitSha = commit.Sha;
                item.CommitDate = commit.Commit.Author.Date.ToString();

                string saveGameInfo = await git.GetCommitContent("test-repo2", $"{Save.Name}/SaveGameInfo", commit.Sha);

                item.SaveService = new SaveFileService();
                item.SaveService.LoadSaveGameInfoXML(saveGameInfo);
                item.Year = item.SaveService.GetYear();
                item.Season = item.SaveService.GetSeason();
                item.Day = item.SaveService.GetDay();

                Save.SaveHistory.Add(item);
            }
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

        Commit latestCommit = await git.GetLatestCommit("test-repo2");

        string commitContent = await git.GetCommitContent("test-repo2", $"{SaveName}/{SaveName}", latestCommit.Sha);

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
