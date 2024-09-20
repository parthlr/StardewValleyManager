using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyManager.ViewModels.Factories;

public class SettingsViewModelFactory : IViewModelFactory<SettingsViewModel>
{
    public SettingsViewModel CreateViewModel()
    {
        return new SettingsViewModel();
    }
}
