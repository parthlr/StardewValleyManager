using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StardewValleyManager.Models;

public partial class PlayerRelationshipModel : ObservableObject
{

    [ObservableProperty]
    private string _npcName = "";

    [ObservableProperty]
    private int _friendshipPoints;

    [ObservableProperty]
    private string _friendshipStatus = "";

}
