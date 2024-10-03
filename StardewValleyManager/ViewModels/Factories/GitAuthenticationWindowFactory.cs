using System;
using StardewValleyManager.Views;

namespace StardewValleyManager.ViewModels.Factories;

public class GitAuthenticationWindowFactory : IWindowFactory<GitAuthenticationWindow>
{
    private readonly IViewModelFactory<GitAuthenticationViewModel> _gitAuthenticationViewModelFactory;

    public GitAuthenticationWindowFactory(IViewModelFactory<GitAuthenticationViewModel> gitAuthenticationViewModelFactory)
    {
        _gitAuthenticationViewModelFactory = gitAuthenticationViewModelFactory;
    }

    public void CreateWindow()
    {
        GitAuthenticationViewModel gitAuthenticationViewModel = _gitAuthenticationViewModelFactory.CreateViewModel();

        GitAuthenticationWindow gitAuthenticationWindow = new GitAuthenticationWindow(gitAuthenticationViewModel);
        gitAuthenticationWindow.Show();
    }

    public void CreateWindow(object param) { }
}
