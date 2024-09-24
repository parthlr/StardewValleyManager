using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels.Factories;

public class SavesViewModelFactory : IViewModelFactory<SavesViewModel>
{
    private readonly GitService _gitService;

    private readonly SettingsService _settingsService;

    private readonly GameSaveFileService _gameSaveFileService;

    public SavesViewModelFactory(GitService gitService, SettingsService settingsService, GameSaveFileService gameSaveFileService)
    {
        _gitService = gitService;
        _settingsService = settingsService;
        _gameSaveFileService = gameSaveFileService;
    }

    public SavesViewModel CreateViewModel()
    {
        return new SavesViewModel(_gitService, _settingsService, _gameSaveFileService);
    }
}
