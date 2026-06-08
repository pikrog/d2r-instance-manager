using System;
using System.Collections.Generic;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Config;

public interface IConfigReader
{
    AppConfig GetAppConfigCopy();
    
    GlobalSettingsSnapshot GetGlobalSettings();

    GameInstanceSnapshot GetInstance(Guid id);
    IReadOnlyList<GameInstanceSnapshot> GetAllInstances();
    bool InstanceExists(Guid id);
    
    RegionSnapshot GetRegion(Guid id);
    IReadOnlyList<RegionSnapshot> GetAllRegions();
    bool RegionExists(Guid id);
    
    AccountSnapshot GetAccount(Guid id);
    IReadOnlyList<AccountSnapshot> GetAllAccounts();
    bool AccountExists(Guid id);
}