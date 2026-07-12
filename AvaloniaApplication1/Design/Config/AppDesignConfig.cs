using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region.Models;

namespace AvaloniaApplication1.Design.Config;

public class AppDesignConfig : AppConfig
{
    [SetsRequiredMembers]
    public AppDesignConfig()
    {
        GlobalSettings = new GlobalSettingsSnapshot();
        Accounts =
        [
            new AccountSnapshot(Guid.NewGuid(),
                "User 1",
                "user1@mail.com",
                "*"),
            new AccountSnapshot(Guid.NewGuid(),
                null,
                "user2@mail.com",
                "*"),
            new AccountSnapshot(Guid.NewGuid(),
                "User 3",
                "user3@mail.com",
                "*"),
        ];

        Regions =
        [
            new RegionSnapshot(Guid.NewGuid(),
                "Europe",
                "eu.actual.battle.net"),
            new RegionSnapshot(Guid.NewGuid(),
                "United States",
                "us.actual.battle.net"),
            new RegionSnapshot(Guid.NewGuid(),
                "Asia",
                "kr.actual.battle.net"),
        ];

        GameInstances =
        [
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Nova",
                true,
                Accounts[0].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Primary(),
                false,
                false,
                new HotKey.HotKey(Key.F1, KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Battle Orders",
                true,
                Accounts[1].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Primary(),
                true,
                true,
                new HotKey.HotKey(Key.F2, KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Mule [EU]",
                true,
                Accounts[2].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Specific("FAKE_DISPLAY_ID_1"),
                true,
                true,
                new HotKey.HotKey(Key.F3, KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Mule [US]",
                true,
                Accounts[2].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[1].Id,
                new DisplaySelection.Specific("FAKE_DISPLAY_ID_2"),
                true,
                true,
                new HotKey.HotKey(Key.F3, KeyModifiers.Control)),
        ];

        Displays = [];
    }
}