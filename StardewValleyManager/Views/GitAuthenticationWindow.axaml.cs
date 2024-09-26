using Avalonia.Controls;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Views;

public partial class GitAuthenticationWindow : Window
{
    public GitAuthenticationWindow(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}