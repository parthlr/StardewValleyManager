using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StardewValleyManager.Services;

public partial class SaveInfoModel : ObservableObject
{
    [ObservableProperty]
    private bool _isEnabled = false;

    [ObservableProperty]
    private bool _isVisible = false;

    [ObservableProperty]
    private bool _isSelected = false;

    [ObservableProperty]
    private string _date = "";

    [ObservableProperty]
    private string _farmName = "";

    [ObservableProperty]
    private string _playerName = "";

    [ObservableProperty]
    private string _farmType = "";

    [ObservableProperty]
    private ObservableCollection<SaveHistoryItemModel> _saveHistory = [];

    public string? Name { get; set; }

    public void LoadFarmName()
    {
        int underscoreIndex = Name.Length - 1;
        for (int i = Name.Length - 1; i >= 0; i--)
        {
            if (Name[i] == '_')
            {
                underscoreIndex = i;
                break;
            }
        }

        FarmName = Name[..underscoreIndex];
    }

}

public partial class SaveHistoryItemModel : ObservableObject
{
    [ObservableProperty]
    private string _commitSha = "";

    [ObservableProperty]
    private string _commitDate = "";

    [ObservableProperty]
    private string _year = "";

    [ObservableProperty]
    private string _season = "";

    [ObservableProperty]
    private string _day = "";
}
