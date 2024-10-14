﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels;

public partial class CommitPropertiesViewModel : ViewModelBase
{
    [ObservableProperty]
    private SaveHistoryItemModel _saveProperties;

    private GitService gitService;

    private SettingsService settingsService;

    public CommitPropertiesViewModel(GitService gitService, SettingsService settingsService)
    {
        this.gitService = gitService;
        this.settingsService = settingsService;
    }

    public void InitializeProperties(object properties)
    {
        SaveProperties = (SaveHistoryItemModel) properties;
    }

    [RelayCommand]
    private void OpenCommitPage()
    {
        string user = settingsService.GetSettingsValue("username");
        string repo = settingsService.GetSettingsValue("repository");

        string commitURL = $"https://github.com/{user}/{repo}/commit/{SaveProperties.CommitSha}";

        Process.Start(new ProcessStartInfo(commitURL) { UseShellExecute = true });
    }

    [RelayCommand]
    private async Task BackupAndLoadCommit()
    {
        // TODO: Add confirmation dialog before call
        await LoadCommit();
    }

    private async Task LoadCommit()
    {
        string saveFolder = SaveProperties.SaveName;

        string user = settingsService.GetSettingsValue("username");
        string repo = settingsService.GetSettingsValue("repository");
        string commit = SaveProperties.CommitSha;

        string saveLocation = settingsService.GetSettingsValue("savesLocation");

        // Backup current local save
        await gitService.CommitSaveFolder(saveFolder);

        // Load commit
        IReadOnlyList<RepositoryContent> downloadContent = await gitService.GetCommitFiles(saveFolder, commit);
        Directory.CreateDirectory($"{saveLocation}/Backup/{saveFolder}");

        foreach (RepositoryContent file in downloadContent)
        {
            // Download each file to the save
            bool downloadResult = await DownloadFromURL(file.DownloadUrl, $"{saveLocation}/Backup/{saveFolder}/{file.Name}");
        }
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
                    }
                    else
                    {
                        return false;
                    }
                }
                else
                {
                    return false;
                }
            }
        }

        return true;
    }
}