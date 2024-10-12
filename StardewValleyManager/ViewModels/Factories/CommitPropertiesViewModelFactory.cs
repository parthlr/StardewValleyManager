using StardewValleyManager.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyManager.ViewModels.Factories;

public class CommitPropertiesViewModelFactory : IViewModelFactory<CommitPropertiesViewModel>
{

    private readonly GitService _gitService;

    private readonly SettingsService _settingsService;

    public CommitPropertiesViewModelFactory(GitService gitService, SettingsService settingsService)
    {
        _gitService = gitService;
        _settingsService = settingsService;
    }

    public CommitPropertiesViewModel CreateViewModel()
    {
        return new CommitPropertiesViewModel(_gitService, _settingsService);
    }
}
