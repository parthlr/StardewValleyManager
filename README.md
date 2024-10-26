# StardewValleyManager

- [Overview](#overview)
  - [Features](#features)
- [Installation](#installation)
  - [Dependencies](#dependencies)
  - [Building From Source](#building-from-source)
- [Basic Usage](#basic-usage)
  - [Authenticating your GitHub Account](#authenticating-your-github-account)
  - [Creating Backups](#creating-backups)
  - [Viewing/Loading Backup Save Points](#viewingloading-backup-save-points)
- [Contributing](#contributing)

## Overview
The **Stardew Valley Save Manager** is a tool for creating and loading backups for Stardew Valley save files on personal GitHub repositories, without the need for providing login details to the app. The idea was to have a centralized place where saves can be stored without managing them manually and locally.

### Features
- Independently backup local save files to GitHub, creating a history of save points
- View information about the local save files and the historical save files
- Load save points stored in GitHub to the local save files (a backup of the current files is created before a save point is loaded)
- Authenticate with a personal GitHub account without providing login information to the app (see: [Device Flow Authentication](https://docs.github.com/en/apps/oauth-apps/building-oauth-apps/authorizing-oauth-apps#device-flow))

## Installation
The app can be downloaded directly from the [latest release](https://github.com/parthlr/StardewValleyManager/releases/latest). Currently, only the `win-x64` runtime is supported for this app. Support for macOS and Linux is planned for the future.

For Windows, only Windows 10 and up is supported. The app may work on Windows 7 and 8, but it is not guaranteed.

### Dependencies
- [.NET 8.0](https://github.com/dotnet/runtime)
- [Avalonia UI](https://github.com/AvaloniaUI/Avalonia)
- [FluentAvalonia](https://github.com/amwx/FluentAvalonia)
- Avalonia.Controls.DataGrid
- CommunityToolkit MVVM
- [Microsoft.Extensions.DependencyInjection](https://www.nuget.org/packages/microsoft.extensions.dependencyinjection/)
- [Octokit](https://github.com/octokit/octokit.net)

### Building from Source
The `build.ps1` script provides a way to build the project based on the `Runtime`, `PublishAot`, and `PublishSingleFile` parameters. These parameters are optional and the `build.ps1` script can be run with the default values.

1. Open PowerShell
2. `git clone https://github.com/parthlr/StardewValleyManager.git`
3. `cd StardewValleyManager`
4. `.\build.ps1 -Runtime <runtime> -PublishAot <true/false> -PublishSingleFile <true/false>`
   - `Runtime`: The target runtime platform. Default: `win-x64`
   - `PublishAot`: Publish the app as Native AOT. Default: `false`
   - `PublishSingleFile`: Builds all application-dependent files into one binary. Default: `false`

At the time, the `Octokit` library for C# .NET does not support Native AOT. I am working on trying to find a workaround for this.

## Basic Usage

### Authenticating your GitHub Account
Create a GitHub account [here](https://github.com/signup). This is the account that will be used to backup local saves. On the app, navigate to the settings page through `Settings > GitHub > Account Setup > Setup Account`.

![settings_view](https://github.com/user-attachments/assets/416a28e1-001b-43f5-b3da-991fe4d70441)

A dialog window should appear with instructions to authenticate your GitHub account. Click the `Generate` button to generate a login code to authenticate your account. Once the code appears, go to [https://github.com/device](https://github.com/device) to enter in your code. You may be asked by GitHub to sign in to your account. **Please note** that the app will not directly ask for your login details. It will open a webpage to the official GitHub website to complete your account authentication. The app will also automatically create a repository for you to store your saves and can be configured to use a custom one in the GitHub settings.

Once you follow the prompted steps on the GitHub website, the dialog window on the app should display a success notification.

![git_auth_view](https://github.com/user-attachments/assets/d6f81fdc-ad7c-4039-a066-b40e7c186d8c)

### Creating Backups
The main page of the app will automatically display farms that are saved locally. Clicking on one will display a table with the local save as well as the backup history on GitHub.

The `Create Save` button will create a backup of the current farm that is being viewed. On the left, the `Save Selected` button will create backups of the selected farms all at once.

![main_view](https://github.com/user-attachments/assets/6611d6d8-0a12-4aa3-9e14-06fca159c21c)

### Viewing/Loading Backup Save Points
Clicking the `View` button on the table will display a dialog window with information about the save. This action can be done even for save points in GitHub. The dialog will display information such as:
- **Properties:** General information about the save (money, year, season, day, etc)
- **Inventory:** Current inventory as of that save point
- **Skills:** the skill level of Farming, Mining, Combat, Foraging, and Fishing
- **Relationships:** Player relationship level with the NPCs as well as relationship status (Friendly/Dating/Married)

The `Open` button will open a webpage to the commit on GitHub. The `Load` button will download the save point and override the local save. Please note that a backup save point of the current local save will be created to GitHub before it is overriden. If the backup fails, the local save will not be overriden. These two buttons are only available for save points backed up to GitHub and not local saves.

![save_history_properties_window](https://github.com/user-attachments/assets/005f4b75-021f-49dd-8079-d9eee920304f)

## Contributing
This project is still a work in progress. This is my first C# .NET application so I am open to feedback, PRs, requests, etc. I started this project as a learning opportunity and wanted to create something that might be useful to some people. There are still a lot of features that I want to implement, and I am trying to improve this application as much as possible, so any help is greatly appreciated.
