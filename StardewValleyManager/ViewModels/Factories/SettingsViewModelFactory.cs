using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValleyManager.Services;
using StardewValleyManager.Views;

namespace StardewValleyManager.ViewModels.Factories;

public class SettingsViewModelFactory : IViewModelFactory<SettingsViewModel>
{
    private readonly SettingsService _settingsService;

    private readonly IWindowFactory<GitAuthenticationWindow> _gitAuthenticationWindowFactory;

    public SettingsViewModelFactory(SettingsService settingsService, IWindowFactory<GitAuthenticationWindow> gitAuthenticationWindowFactory)
    {
        _settingsService = settingsService;
        _gitAuthenticationWindowFactory = gitAuthenticationWindowFactory;
    }

    public SettingsViewModel CreateViewModel()
    {
        return new SettingsViewModel(_settingsService, _gitAuthenticationWindowFactory);
    }
}
