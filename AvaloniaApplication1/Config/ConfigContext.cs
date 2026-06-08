using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using AvaloniaApplication1.Exceptions;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Config;

// todo: add/update/remove integrity checks. uniqueness, etc.
public class ConfigContext(AppConfig appConfig) : IConfigReader
{
    private readonly AppConfig _appConfig = appConfig;

    public AppConfig GetAppConfigCopy()
    {
        return new AppConfig
        {
            GlobalSettings = _appConfig.GlobalSettings,
            GameInstances = [.. _appConfig.GameInstances],
            Regions = [.. _appConfig.Regions],
            Accounts = [.. _appConfig.Accounts]
        };
    }
    
    #region GlobalSettings
    public GlobalSettingsSnapshot GetGlobalSettings()
    {
        return _appConfig.GlobalSettings;
    }
    
    public void UpdateGlobalSettings(GlobalSettingsSnapshot snapshot)
    {
        _appConfig.GlobalSettings = snapshot;
    }
    #endregion
    
    #region Instances
    public void AddInstance(GameInstanceSnapshot snapshot)
    {
        _appConfig.GameInstances.Add(snapshot);
    }
    
    public void UpdateInstance(GameInstanceSnapshot snapshot)
    {
        var index = _appConfig.GameInstances.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Instance with id {snapshot.Id} not found");
        _appConfig.GameInstances[index] = snapshot;
    }

    public void RemoveInstance(Guid id)
    {
        _appConfig.GameInstances.RemoveAll(i => i.Id == id);
    }

    public GameInstanceSnapshot GetInstance(Guid id)
    {
        var instance = _appConfig.GameInstances.Find(i => i.Id == id);
        return instance ?? throw new ConfigNotFoundException($"Instance with id {id} not found");
    }

    public IReadOnlyList<GameInstanceSnapshot> GetAllInstances()
    {
        return _appConfig.GameInstances.AsReadOnly();
    }

    public bool InstanceExists(Guid id)
    {
        return _appConfig.GameInstances.Exists(i => i.Id == id);
    }
    #endregion
    
    #region Regions
    public void AddRegion(RegionSnapshot snapshot)
    {
        _appConfig.Regions.Add(snapshot);
    }

    public void UpdateRegion(RegionSnapshot snapshot)
    {
        var index = _appConfig.Regions.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Region with id {snapshot.Id} not found");
        _appConfig.Regions[index] = snapshot;
    }
    
    public void RemoveRegion(Guid id)
    {
        _appConfig.Regions.RemoveAll(i => i.Id == id);
    }

    public RegionSnapshot GetRegion(Guid id)
    {
        var region = _appConfig.Regions.Find(i => i.Id == id);
        return region ?? throw new ConfigNotFoundException($"Region with id {id} not found");
    }
    
    public IReadOnlyList<RegionSnapshot> GetAllRegions()
    {
        return _appConfig.Regions.AsReadOnly();
    }
    
    public bool RegionExists(Guid id)
    {
        return _appConfig.Regions.Exists(i => i.Id == id);
    }
    #endregion
    
    #region Accounts
    public void AddAccount(AccountSnapshot snapshot)
    {
        _appConfig.Accounts.Add(snapshot);
    }

    public void UpdateAccount(AccountSnapshot snapshot)
    {
        var index = _appConfig.Accounts.FindIndex(i => i.Id == snapshot.Id);
        if (index == -1)
            throw new ConfigNotFoundException($"Account with id {snapshot.Id} not found");
        _appConfig.Accounts[index] = snapshot;
    }
    
    public void RemoveAccount(Guid id)
    {
        _appConfig.Accounts.RemoveAll(i => i.Id == id);
    }
    
    public AccountSnapshot GetAccount(Guid id)
    {
        var account = _appConfig.Accounts.Find(i => i.Id == id);
        return account ?? throw new ConfigNotFoundException($"Account with id {id} not found");
    }
    
    public IReadOnlyList<AccountSnapshot> GetAllAccounts()
    {
        return _appConfig.Accounts.AsReadOnly();
    }
    
    public bool AccountExists(Guid id)
    {
        return _appConfig.Accounts.Exists(i => i.Id == id);
    }
    #endregion
}