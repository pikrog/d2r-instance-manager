using System;
using System.Collections.Generic;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Display;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Region.Models;

namespace D2RInstanceManager.Config;

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
    
    DisplaySnapshot GetDisplay(string id);
    bool DisplayExists(string id);
}