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

    public SavesViewModelFactory(GitService gitService)
    {
        _gitService = gitService;
    }

    public SavesViewModel CreateViewModel()
    {
        return new SavesViewModel(_gitService);
    }
}
