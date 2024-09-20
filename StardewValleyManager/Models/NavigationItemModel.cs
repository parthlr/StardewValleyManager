using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Models;

public partial class NavigationItemModel : ObservableObject
{

    [ObservableProperty]
    private string _title;

    [ObservableProperty]
    private string _icon;

    [ObservableProperty]
    private ViewModelBase _viewModel;

    public NavigationItemModel(string title, string icon, ViewModelBase viewModel)
    {
        Title = title;
        Icon = icon;
        ViewModel = viewModel;
    }
}
