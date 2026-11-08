using System;
using Avalonia.Input;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Design.Services;
using D2RInstanceManager.Display;
using D2RInstanceManager.HotKey;
using D2RInstanceManager.Instance.ViewModels;
using D2RInstanceManager.Region.Models;

namespace D2RInstanceManager.Design.ViewModels;

public class EditInstanceFormDesignViewModel : EditInstanceFormViewModel
{
    public EditInstanceFormDesignViewModel() : base(new FakeHotKeyConfigValidator(),
        [
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
        new DisplayOption.Specific(1, new DisplaySnapshot("FAKE_DISPLAY_ID_1", "MSI X01", 1920, 1080), true),
        new DisplayOption.Specific(2, new DisplaySnapshot("FAKE_DISPLAY_ID_2", "LG Y02", 1440, 900), true)
    ])
    {
        Name = "Hammer";
        IsOnlineMode = true;
        SelectedAccount = Accounts[0];
        SelectedRegion = Regions[0];
        SelectedDisplay = Displays[0];
        ShowCommandHotKey = new KeyCombination(Key.F1, KeyModifiers.Control);
        IsNoSound = true;
    }
}


