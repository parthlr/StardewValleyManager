using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using StardewValleyManager.Models;
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
    private string _farmTypeIconPath = null;

    [ObservableProperty]
    private string _petIconPath = null;

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
    // General properties
    [ObservableProperty]
    private bool _isLocalSave = false;

    [ObservableProperty]
    private string _saveName = "";

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

    [ObservableProperty]
    private int _money = 0;

    [ObservableProperty]
    private int _totalMoneyEarned = 0;

    // NPC relationship status
    [ObservableProperty]
    private ObservableCollection<PlayerRelationshipModel> _relationshipStatus;

    // Skill levels
    [ObservableProperty]
    private int _farmingLevel = 0;

    [ObservableProperty]
    private int _miningLevel = 0;

    [ObservableProperty]
    private int _combatLevel = 0;

    [ObservableProperty]
    private int _foragingLevel = 0;

    [ObservableProperty]
    private int _fishingLevel = 0;

    // Player inventory
    [ObservableProperty]
    private ObservableCollection<InventoryItemModel> _playerInventory;
}
