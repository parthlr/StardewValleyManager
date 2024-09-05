using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;

public partial class SaveInfoModel : ObservableObject
{
    [ObservableProperty]
    private string _date = "";

    [ObservableProperty]
    private string _farmName = "";

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
    private string _commitDate = "";
}
