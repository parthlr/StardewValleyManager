using System;
using System.Collections.ObjectModel;
using System.Windows.Markup;
using CommunityToolkit.Mvvm.ComponentModel;
using StardewValleyManager.Models;
using StardewValleyManager.ViewModels.Factories;

namespace StardewValleyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private ViewModelBase _currentPage;

    [ObservableProperty]
    private NavigationItemModel _selectedView;

    private readonly IViewModelFactory<SavesViewModel> _savesViewModelFactory;

    private readonly IViewModelFactory<SettingsViewModel> _settingsViewModelFactory;

    [ObservableProperty]
    ObservableCollection<NavigationItemModel> _menuViewModels;

    [ObservableProperty]
    ObservableCollection<NavigationItemModel> _footerViewModels;

    public MainViewModel(IViewModelFactory<SavesViewModel> savesViewModelFactory, IViewModelFactory<SettingsViewModel> settingsViewModelFactory)
    {
        _savesViewModelFactory = savesViewModelFactory;
        _settingsViewModelFactory = settingsViewModelFactory;

        MenuViewModels = new()
        {
            new NavigationItemModel("Home", "Home", _savesViewModelFactory.CreateViewModel())
        };

        FooterViewModels = new()
        {
            new NavigationItemModel("Settings", "Settings", _settingsViewModelFactory.CreateViewModel())
        };

        SelectedView = MenuViewModels[0];
        CurrentPage = MenuViewModels[0].ViewModel;
    }

    partial void OnSelectedViewChanged(NavigationItemModel value)
    {
        if (value != null)
        {
            CurrentPage = value.ViewModel;
        }
    }
}
