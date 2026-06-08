using System.IO;
using System.Threading.Tasks;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Config;

public class ConfigLoader(IConfigStore configStore)
{
    private static AppConfig CreateDefault()
    {
        return new AppConfig
        {
            GlobalSettings = new GlobalSettingsSnapshot(),
            Accounts = [],
            Regions =
            [
                new RegionSnapshot(ConfigConstants.EuropeRegionId,
                    ConfigConstants.EuropeRegionName,
                    ConfigConstants.EuropeRegionAddress),
                new RegionSnapshot(ConfigConstants.UnitedStatesRegionId,
                    ConfigConstants.UnitedStatesRegionName,
                    ConfigConstants.UnitedStatesRegionAddress),
                new RegionSnapshot(ConfigConstants.AsiaRegionId,
                    ConfigConstants.AsiaRegionName,
                    ConfigConstants.AsiaRegionAddress)
            ],
            GameInstances = [],
        };
    }
    
    private void Validate(AppConfig config)
    {
        // todo: validate config
        //throw new NotImplementedException();
    }
    
    public async Task<AppConfig> LoadOrCreateDefaultAsync()
    {
        AppConfig config;
        try
        {
            config = await configStore.LoadAsync();
        }
        catch (IOException e) when (e is FileNotFoundException or DirectoryNotFoundException)
        {
            config = CreateDefault();
            await configStore.SaveAsync(config);
        }
        Validate(config);
        return config;
    }
}