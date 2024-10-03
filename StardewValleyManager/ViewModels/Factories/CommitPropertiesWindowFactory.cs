using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using StardewValleyManager.Views;

namespace StardewValleyManager.ViewModels.Factories;

public class CommitPropertiesWindowFactory : IWindowFactory<CommitPropertiesWindow>
{

    private readonly IViewModelFactory<CommitPropertiesViewModel> _commitPropertiesViewModelFactory;

    public CommitPropertiesWindowFactory(IViewModelFactory<CommitPropertiesViewModel> commitPropertiesViewModelFactory)
    {
        _commitPropertiesViewModelFactory = commitPropertiesViewModelFactory;
    }

    public void CreateWindow()
    {
        
    }

    public void CreateWindow(object saveProperties)
    {
        CommitPropertiesViewModel commitPropertiesViewModel = _commitPropertiesViewModelFactory.CreateViewModel();
        commitPropertiesViewModel.InitializeProperties(saveProperties);

        CommitPropertiesWindow window = new CommitPropertiesWindow(commitPropertiesViewModel);
        window.Show();
    }
}
