using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyManager.ViewModels.Factories;

public class CommitPropertiesViewModelFactory : IViewModelFactory<CommitPropertiesViewModel>
{
    public CommitPropertiesViewModel CreateViewModel()
    {
        return new CommitPropertiesViewModel();
    }
}
