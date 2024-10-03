using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace StardewValleyManager.ViewModels;

public partial class CommitPropertiesViewModel : ViewModelBase
{
    [ObservableProperty]
    private SaveHistoryItemModel _saveProperties;

    public CommitPropertiesViewModel()
    {

    }

    public void InitializeProperties(object properties)
    {
        SaveProperties = (SaveHistoryItemModel) properties;
    }

}
