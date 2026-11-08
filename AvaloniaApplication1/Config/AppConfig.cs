using System.Collections.Generic;
using D2RInstanceManager.Account.Models;
using D2RInstanceManager.Display;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Region.Models;

namespace D2RInstanceManager.Config;

public class AppConfig
{
    public required GlobalSettingsSnapshot GlobalSettings { get; set; }
    public required List<AccountSnapshot> Accounts { get; set; }
    public required List<RegionSnapshot> Regions { get; set; }
    public required List<InstanceSnapshot> Instances { get; set; }
    public required List<DisplaySnapshot> Displays { get; set; }
}