using System;
using System.Collections.Generic;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region.Models;

namespace AvaloniaApplication1.Config;

public interface IConfigReader
{
    AppConfig GetAppConfigCopy();
    
    GlobalSettingsSnapshot GetGlobalSettings();

    InstanceSnapshot GetInstance(Guid id);
    IReadOnlyList<InstanceSnapshot> GetAllInstances();
    bool InstanceExists(Guid id);
    
    RegionSnapshot GetRegion(Guid id);
    IReadOnlyList<RegionSnapshot> GetAllRegions();
    bool RegionExists(Guid id);
    
    AccountSnapshot GetAccount(Guid id);
    IReadOnlyList<AccountSnapshot> GetAllAccounts();
    bool AccountExists(Guid id);
    
    CachedDisplaySnapshot GetCachedDisplay(string id);
}