using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using StardewValleyManager.Models;

namespace StardewValleyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private NavigationItemModel _selectedView;

    [ObservableProperty]
    ObservableCollection<NavigationItemModel> _menuViewModels = new()
    {
        new NavigationItemModel("Home", "Home", new SavesViewModel())
    };

    [ObservableProperty]
    ObservableCollection<NavigationItemModel> _footerViewModels = new()
    {
        new NavigationItemModel("Settings", "Settings", new SettingsViewModel())
    };

    public MainViewModel()
    {
        SelectedView = MenuViewModels[0];
        SelectedView.Initialized = true;
    }

    partial void OnSelectedViewChanged(NavigationItemModel value)
    {
        if (value != null)
        {
            if (!value.Initialized)
            {
                object viewModelInstance = Activator.CreateInstance(value.ViewModel.GetType());
                if (viewModelInstance == null)
                {
                    return;
                }

                value.Initialized = true;
                CurrentPage = (ViewModelBase)viewModelInstance;
            } else
            {
                CurrentPage = value.ViewModel;
            }
        }
    }
}
