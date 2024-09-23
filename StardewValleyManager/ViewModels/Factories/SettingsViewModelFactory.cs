using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels.Factories;

public class SettingsViewModelFactory : IViewModelFactory<SettingsViewModel>
{
    private readonly SettingsService _settingsService;

    public SettingsViewModelFactory(SettingsService settingsService)
    {
        _settingsService = settingsService;
    }

    public SettingsViewModel CreateViewModel()
    {
        return new SettingsViewModel(_settingsService);
    }
}
