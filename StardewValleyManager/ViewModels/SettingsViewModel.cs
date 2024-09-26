using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Avalonia;
using Avalonia.Styling;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using StardewValleyManager.Services;
using StardewValleyManager.ViewModels.Factories;
using StardewValleyManager.Views;

namespace StardewValleyManager.ViewModels;

public partial class SettingsViewModel : ViewModelBase
{
    [ObservableProperty]
    private string _appTheme;

    [ObservableProperty]
    private string[] _themeList = ["System", "Dark", "Light"];

    [ObservableProperty]
    private string _savesLocation;

    [ObservableProperty]
    private string _gitToken;

    [ObservableProperty]
    private string _gitUsername;

    [ObservableProperty]
    private string _repositoryName;

    private SettingsService settingsService;

    private IWindowFactory<GitAuthenticationWindow> gitAuthenticationWindowFactory;

    public SettingsViewModel(SettingsService settingsService, IWindowFactory<GitAuthenticationWindow> gitAuthenticationWindowFactory)
    {
        this.settingsService = settingsService;
        this.gitAuthenticationWindowFactory = gitAuthenticationWindowFactory;

        ReadSettings();
    }

    public void ReadSettings()
    {
        AppTheme = settingsService.GetSettingsValue("theme");
        SavesLocation = settingsService.GetSettingsValue("savesLocation");
        GitToken = settingsService.GetSettingsValue("gitToken");
        GitUsername = settingsService.GetSettingsValue("username");
        RepositoryName = settingsService.GetSettingsValue("repository");
    }

    [RelayCommand]
    private void OpenGitAuthenticationWindow()
    {
        gitAuthenticationWindowFactory.CreateWindow();
    }

    [RelayCommand]
    public void SaveSettings()
    {
        settingsService.SaveSettings();
    }

    private ThemeVariant GetThemeVariant(string theme)
    {
        switch (theme)
        {
            case "System":
                return ThemeVariant.Default;
            case "Dark":
                return ThemeVariant.Dark;
            case "Light":
                return ThemeVariant.Light;
            default:
                return null;
        }
    }

    partial void OnAppThemeChanged(string value)
    {
        ThemeVariant newTheme = GetThemeVariant(value);
        if (newTheme != null)
        {
            Application.Current.RequestedThemeVariant = newTheme;
        }

        settingsService.UpdateSettingsValue("theme", AppTheme, true);
    }

    partial void OnRepositoryNameChanged(string value)
    {
        settingsService.UpdateSettingsValue("repository", RepositoryName, true);
    }

}
