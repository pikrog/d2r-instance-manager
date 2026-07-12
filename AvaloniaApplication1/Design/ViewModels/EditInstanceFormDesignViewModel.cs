using System;
using Avalonia.Input;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Region.Models;

namespace AvaloniaApplication1.Design.ViewModels;

public class EditInstanceFormDesignViewModel : EditInstanceFormViewModel
{
    public EditInstanceFormDesignViewModel() : base([
        new AccountOption(Guid.NewGuid(), "User 1", "user1@mail.com"),
        new AccountOption(Guid.NewGuid(), null, "user2@mail.com"), 
        new AccountOption(Guid.NewGuid(), "User 3", "user3@mail.com")
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
        RecallHotKey = new HotKey.HotKey(Key.F1, KeyModifiers.Control);
        IsNoSound = true;
    }
}


