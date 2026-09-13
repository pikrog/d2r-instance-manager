using System.IO;
using System.Threading.Tasks;
using AvaloniaApplication1.Config.Stores;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Region.Models;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Config;

public class ConfigLoader(IConfigStore configStore, ILogger<ConfigLoader> logger)
{
    private static AppConfig CreateDefault()
    {
        return new AppConfig
        {
            GlobalSettings = new GlobalSettingsSnapshot(),
            Accounts = [],
            Regions =
            [
                new RegionSnapshot(ConfigDefaults.EuropeRegionId,
                    ConfigDefaults.EuropeRegionName,
                    ConfigDefaults.EuropeRegionAddress),
                new RegionSnapshot(ConfigDefaults.UnitedStatesRegionId,
                    ConfigDefaults.UnitedStatesRegionName,
                    ConfigDefaults.UnitedStatesRegionAddress),
                new RegionSnapshot(ConfigDefaults.AsiaRegionId,
                    ConfigDefaults.AsiaRegionName,
                    ConfigDefaults.AsiaRegionAddress)
            ],
            Instances = [],
            Displays = [],
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
            
            logger.LogInformation("Created default config");
            
            await configStore.SaveAsync(config);
        }
        Validate(config);
        return config;
    }
}