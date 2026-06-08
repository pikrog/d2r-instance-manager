using System;
using Avalonia.Input;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.ViewModels;

namespace AvaloniaApplication1.Design.ViewModels;

public class EditInstanceFormDesignViewModel : EditInstanceFormViewModel
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
    [1, 2])
    {
        Name = "Hammer";
        IsOnlineMode = true;
        SelectedAccount = Accounts[0];
        SelectedRegion = Regions[0];
        Displays = [1, 2];
        Display = 1;
        RecallHotKey = new HotKey(Key.F1, KeyModifiers.Control);
        IsNoSound = true;
    }
}


