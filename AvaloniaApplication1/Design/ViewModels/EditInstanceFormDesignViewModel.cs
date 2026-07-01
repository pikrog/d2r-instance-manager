using System;
using Avalonia.Input;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;
using EditInstanceFormViewModel = AvaloniaApplication1.ViewModels.EditInstanceFormViewModel;

namespace AvaloniaApplication1.Design.ViewModels;

public class EditInstanceFormDesignViewModel : AvaloniaApplication1.ViewModels.EditInstanceFormViewModel
{
    public EditInstanceFormDesignViewModel() : base([
        new AccountOption(Guid.NewGuid(), "user1"),
        new AccountOption(Guid.NewGuid(), "user2"), 
        new AccountOption(Guid.NewGuid(), "user3")
    ],
    [
        new RegionOption(Guid.NewGuid(), "Europe"),
        new RegionOption(Guid.NewGuid(), "Asia"),
        new RegionOption(Guid.NewGuid(), "United States")
    ],
    [
        new DisplayOption.Primary(),
        new DisplayOption.Specific(1, "FAKE_DISPLAY_ID_1", "MSI X01", 1920, 1080, true),
        new DisplayOption.Specific(2, "FAKE_DISPLAY_ID_2", "LG Y02", 1440, 900, true)
    ])
    {
        Name = "Hammer";
        IsOnlineMode = true;
        SelectedAccount = Accounts[0];
        SelectedRegion = Regions[0];
        SelectedDisplay = Displays[0];
        RecallHotKey = new HotKey(Key.F1, KeyModifiers.Control);
        IsNoSound = true;
    }
}


