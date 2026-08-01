using System.Collections.Generic;
using AvaloniaApplication1.Account.Models;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region.Models;

namespace AvaloniaApplication1.Config;

public class AppConfig
{
    public required GlobalSettingsSnapshot GlobalSettings { get; set; }
    public required List<AccountSnapshot> Accounts { get; set; }
    public required List<RegionSnapshot> Regions { get; set; }
    public required List<InstanceSnapshot> Instances { get; set; }
    public required List<CachedDisplaySnapshot> Displays { get; set; }
}