using System;
using System.Collections.Generic;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Config.Exceptions;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region.Models;

namespace AvaloniaApplication1.Config;

// todo: add/update/remove integrity checks. uniqueness, etc.
public class ConfigContext(AppConfig appConfig) : IConfigReader
{
    public AppConfig GetAppConfigCopy()
    {
        return new AppConfig
        {
            GlobalSettings = appConfig.GlobalSettings,
            GameInstances = [.. appConfig.GameInstances],
            Regions = [.. appConfig.Regions],
            Accounts = [.. appConfig.Accounts],
            Displays = [.. appConfig.Displays]
        };
    }
    
    #region GlobalSettings
    public GlobalSettingsSnapshot GetGlobalSettings()
    {
        return appConfig.GlobalSettings;
    }
    
    public void UpdateGlobalSettings(GlobalSettingsSnapshot snapshot)
    {
        appConfig.GlobalSettings = snapshot;
    }
    #endregion
    
    #region Instances
    public void AddInstance(GameInstanceSnapshot snapshot)
    {
        appConfig.GameInstances.Add(snapshot);
    }
    
    public void UpdateInstance(GameInstanceSnapshot snapshot)
    {
        var index = appConfig.GameInstances.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Instance with id {snapshot.Id} not found");
        appConfig.GameInstances[index] = snapshot;
    }

    public void RemoveInstance(Guid id)
    {
        appConfig.GameInstances.RemoveAll(i => i.Id == id);
    }

    public GameInstanceSnapshot GetInstance(Guid id)
    {
        var instance = appConfig.GameInstances.Find(i => i.Id == id);
        return instance ?? throw new ConfigNotFoundException($"Instance with id {id} not found");
    }

    public IReadOnlyList<GameInstanceSnapshot> GetAllInstances()
    {
        return appConfig.GameInstances.AsReadOnly();
    }

    public bool InstanceExists(Guid id)
    {
        return appConfig.GameInstances.Exists(i => i.Id == id);
    }
    #endregion
    
    #region Regions
    public void AddRegion(RegionSnapshot snapshot)
    {
        appConfig.Regions.Add(snapshot);
    }

    public void UpdateRegion(RegionSnapshot snapshot)
    {
        var index = appConfig.Regions.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Region with id {snapshot.Id} not found");
        appConfig.Regions[index] = snapshot;
    }
    
    public void RemoveRegion(Guid id)
    {
        appConfig.Regions.RemoveAll(i => i.Id == id);
    }

    public RegionSnapshot GetRegion(Guid id)
    {
        var region = appConfig.Regions.Find(i => i.Id == id);
        return region ?? throw new ConfigNotFoundException($"Region with id {id} not found");
    }
    
    public IReadOnlyList<RegionSnapshot> GetAllRegions()
    {
        return appConfig.Regions.AsReadOnly();
    }
    
    public bool RegionExists(Guid id)
    {
        return appConfig.Regions.Exists(i => i.Id == id);
    }
    #endregion
    
    #region Accounts
    public void AddAccount(AccountSnapshot snapshot)
    {
        appConfig.Accounts.Add(snapshot);
    }

    public void UpdateAccount(AccountSnapshot snapshot)
    {
        var index = appConfig.Accounts.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Account with id {snapshot.Id} not found");
        appConfig.Accounts[index] = snapshot;
    }
    
    public void RemoveAccount(Guid id)
    {
        appConfig.Accounts.RemoveAll(i => i.Id == id);
    }
    
    public AccountSnapshot GetAccount(Guid id)
    {
        var account = appConfig.Accounts.Find(i => i.Id == id);
        return account ?? throw new ConfigNotFoundException($"Account with id {id} not found");
    }
    
    public IReadOnlyList<AccountSnapshot> GetAllAccounts()
    {
        return appConfig.Accounts.AsReadOnly();
    }
    
    public bool AccountExists(Guid id)
    {
        return appConfig.Accounts.Exists(i => i.Id == id);
    }
    #endregion
    
    #region Displays
    public CachedDisplaySnapshot GetCachedDisplay(string id)
    {
        var display = appConfig.Displays.Find(i => i.Id == id);
        return display ?? throw new ConfigNotFoundException($"Display with id {id} not found");
    }
    
    public void CacheDisplay(CachedDisplaySnapshot snapshot)
    {
        var index = appConfig.Displays.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
        {
            appConfig.Displays.Add(snapshot);
            return;
        }
        appConfig.Displays[index] = snapshot;
    }
    #endregion
}