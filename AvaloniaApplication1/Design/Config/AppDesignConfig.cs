using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Snapshots;

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
                "*"),
            new AccountSnapshot(Guid.NewGuid(),
                "User 2",
                "*"),
            new AccountSnapshot(Guid.NewGuid(),
                "User 3",
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
                CredentialsVector.OsiTokenRegistry,
                Regions[0].Id,
                1,
                false,
                false,
                new HotKey(Key.F1,
                    KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Battle Orders",
                true,
                Accounts[1].Id,
                CredentialsVector.OsiTokenRegistry,
                Regions[0].Id,
                2,
                true,
                true,
                new HotKey(Key.F2,
                    KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Mule [EU]",
                true,
                Accounts[2].Id,
                CredentialsVector.OsiTokenRegistry,
                Regions[0].Id,
                1,
                true,
                true,
                new HotKey(Key.F3,
                    KeyModifiers.Control)),
            new GameInstanceSnapshot(Guid.NewGuid(),
                "Mule [US]",
                true,
                Accounts[2].Id,
                CredentialsVector.OsiTokenRegistry,
                Regions[1].Id,
                1,
                true,
                true,
                new HotKey(Key.F3,
                    KeyModifiers.Control)),
        ];
    }
}