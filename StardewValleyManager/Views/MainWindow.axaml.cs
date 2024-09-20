using Avalonia.Controls;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Views;

public partial class MainWindow : Window
{
    public MainWindow(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
        //MainView.DataContext = new MainViewModel();
    }
}