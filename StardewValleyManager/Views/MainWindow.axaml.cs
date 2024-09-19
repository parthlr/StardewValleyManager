using Avalonia.Controls;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        MainView.DataContext = new MainViewModel();
    }
}