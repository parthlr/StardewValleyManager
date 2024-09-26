using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValleyManager.Services;

namespace StardewValleyManager.ViewModels.Factories;

public class GitAuthenticationViewModelFactory : IViewModelFactory<GitAuthenticationViewModel>
{
    private readonly GitService _gitService;

    private readonly SettingsService _settingsService;

    public GitAuthenticationViewModelFactory(GitService gitService, SettingsService settingsService)
    {
        _gitService = gitService;
        _settingsService = settingsService;
    }

    public GitAuthenticationViewModel CreateViewModel()
    {
        return new GitAuthenticationViewModel(_gitService, _settingsService);
    }
}
