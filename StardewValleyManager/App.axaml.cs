using System;
using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Data.Core;
using Avalonia.Data.Core.Plugins;
using Avalonia.Markup.Xaml;
using Microsoft.Extensions.DependencyInjection;
using StardewValleyManager.Services;
using StardewValleyManager.ViewModels;
using StardewValleyManager.ViewModels.Factories;
using StardewValleyManager.Views;

namespace StardewValleyManager;

public partial class App : Application
{
    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        IServiceProvider serviceProvider = CreateServiceProvider();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            // Line below is needed to remove Avalonia data validation.
            // Without this line you will get duplicate validations from both Avalonia and CT
            BindingPlugins.DataValidators.RemoveAt(0);
            desktop.MainWindow = serviceProvider.GetService<MainWindow>();
        }
        else if (ApplicationLifetime is ISingleViewApplicationLifetime singleViewPlatform)
        {
            singleViewPlatform.MainView = new MainView
            {
                DataContext = serviceProvider.GetRequiredService<MainViewModel>()
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private IServiceProvider CreateServiceProvider()
    {
        IServiceCollection services = new ServiceCollection();

        services.AddSingleton<GitService>();
        services.AddSingleton<GameSaveFileService>();
        services.AddSingleton<SettingsService>();

        services.AddSingleton<IViewModelFactory<SavesViewModel>, SavesViewModelFactory>();
        services.AddSingleton<IViewModelFactory<SettingsViewModel>, SettingsViewModelFactory>();
        services.AddSingleton<IViewModelFactory<GitAuthenticationViewModel>, GitAuthenticationViewModelFactory>();

        services.AddSingleton<IWindowFactory<GitAuthenticationWindow>, GitAuthenticationWindowFactory>();

        services.AddScoped<MainViewModel>();

        services.AddScoped<MainWindow>(s => new MainWindow(s.GetRequiredService<MainViewModel>()));

        return services.BuildServiceProvider();
    }
}