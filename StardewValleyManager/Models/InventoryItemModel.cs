using CommunityToolkit.Mvvm.ComponentModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StardewValleyManager.Models;
public partial class InventoryItemModel : ObservableObject
{

    [ObservableProperty]
    private string _name = "";

    [ObservableProperty]
    private int _stackCount;

    [ObservableProperty]
    private string _quality = "Normal";

    [ObservableProperty]
    private string _price = "-";

}
