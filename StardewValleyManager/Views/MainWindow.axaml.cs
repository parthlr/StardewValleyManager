using Avalonia.Controls;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
        SaveFiles.DataContext = new SavesViewModel();
    }
}