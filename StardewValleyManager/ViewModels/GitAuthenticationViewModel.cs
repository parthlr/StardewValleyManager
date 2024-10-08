﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Octokit;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels;

public partial class GitAuthenticationViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _userCode = "";

    [ObservableProperty]
    private bool _isAuthenticated = false;

    private GitService git;

    private SettingsService settingsService;

    public GitAuthenticationViewModel(GitService gitService, SettingsService settingsService)
    {
        git = gitService;
        this.settingsService = settingsService;
    }

    [RelayCommand]
    private async Task GenerateUserCodeAndWaitForAuthentication()
    {
        OauthDeviceFlowResponse authResponse = await git.GenerateUserCode();

        UserCode = authResponse.UserCode;

        string gitToken = await git.GetUserAuthToken(authResponse);
        git.AuthToken = gitToken;
        git.InitCredentials();

        string username = await git.GetUserLogin();
        git.User = username;

        settingsService.UpdateSettingsValue("gitToken", gitToken, true);
        settingsService.UpdateSettingsValue("username", username, true);

        IsAuthenticated = true;
    }

    [RelayCommand]
    private void OpenWebpage(string url)
    {
        Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
    }
}
