using CommunityToolkit.Mvvm.ComponentModel;

namespace StardewValleyManager.ViewModels;

public partial class MainViewModel : ObservableObject
{
    [ObservableProperty]
    private string _greeting = "Welcome to Avalonia!";
}
