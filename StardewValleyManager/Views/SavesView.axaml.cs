using Avalonia;
using Avalonia.Collections;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using FluentAvalonia.UI.Controls;
using StardewValleyManager.ViewModels;
using System.Collections.Generic;
using System.ComponentModel;

namespace StardewValleyManager.Views;

public partial class SavesView : UserControl
{
    public SavesView()
    {
        InitializeComponent();
    }

    protected override void OnLoaded(RoutedEventArgs e)
    {
        base.OnLoaded(e);

        SaveHistoryTable.ItemsSource = new DataGridCollectionView(SaveHistoryTable.ItemsSource)
        {
            GroupDescriptions =
            {
                new DataGridPathGroupDescription("SaveSource")
            }
        };
    }

    private void OpenSaveDetails(object? sender, RoutedEventArgs e)
    {
        Dispatcher.UIThread.Post(async () =>
        {
            await (DataContext as SavesViewModel).LoadSaveHistoryCommand.ExecuteAsync((e.Source as SettingsExpander).CommandParameter);
            SaveHistoryTable.ItemsSource = new DataGridCollectionView(SaveHistoryTable.ItemsSource)
            {
                GroupDescriptions =
                {
                    new DataGridPathGroupDescription("SaveSource")
                }
            };
        }, DispatcherPriority.Send);
    }
}