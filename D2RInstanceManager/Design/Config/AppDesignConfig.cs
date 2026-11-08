using System;
using System.Diagnostics.CodeAnalysis;
using Avalonia.Input;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Authentication.Models;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.HotKey;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Region.Models;
using D2RInstanceManager.Secret.Data;

namespace D2RInstanceManager.Design.Config;

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
                new SecretData([])),
            new AccountSnapshot(Guid.NewGuid(),
                null,
                "user2@mail.com",
                new SecretData([])),
            new AccountSnapshot(Guid.NewGuid(),
                "User 3",
                "user3@mail.com",
                new SecretData([])),
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

        Instances =
        [
            new InstanceSnapshot(Guid.NewGuid(),
                "Nova",
                true,
                Accounts[0].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Primary(),
                false,
                false,
                new KeyCombination(Key.F1, KeyModifiers.Control)),
            new InstanceSnapshot(Guid.NewGuid(),
                "Battle Orders",
                true,
                Accounts[1].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Primary(),
                true,
                true,
                new KeyCombination(Key.F2, KeyModifiers.Control)),
            new InstanceSnapshot(Guid.NewGuid(),
                "Mule [EU]",
                true,
                Accounts[2].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[0].Id,
                new DisplaySelection.Specific("FAKE_DISPLAY_ID_1"),
                true,
                true,
                new KeyCombination(Key.F3, KeyModifiers.Control)),
            new InstanceSnapshot(Guid.NewGuid(),
                "Mule [US]",
                true,
                Accounts[2].Id,
                AuthenticationMethod.OsiTokenRegistry,
                Regions[1].Id,
                new DisplaySelection.Specific("FAKE_DISPLAY_ID_2"),
                true,
                true,
                new KeyCombination(Key.F3, KeyModifiers.Control)),
        ];

        Displays = [];
    }
}