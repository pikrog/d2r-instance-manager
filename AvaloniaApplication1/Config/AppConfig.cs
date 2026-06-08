using System.Collections.Generic;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Config;

public class AppConfig
{
    public required GlobalSettingsSnapshot GlobalSettings { get; set; }
    public required List<AccountSnapshot> Accounts { get; set; }
    public required List<RegionSnapshot> Regions { get; set; }
    public required List<GameInstanceSnapshot> GameInstances { get; set; }
}