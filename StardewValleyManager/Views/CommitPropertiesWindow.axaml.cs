using Avalonia;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using StardewValleyManager.ViewModels;

namespace StardewValleyManager.Views;

public partial class CommitPropertiesWindow : Window
{
    public CommitPropertiesWindow(object dataContext)
    {
        InitializeComponent();
        DataContext = dataContext;
    }
}