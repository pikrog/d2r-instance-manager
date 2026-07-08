using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Config.Stores;

namespace AvaloniaApplication1.Bootstrap;

public static class ConfigBootstrapper
{
    public static Task<CoreConfigServicesBundle> BootstrapAsync()
    {
        var appEnvironment = ConfigEnvironment.CreateInApplicationDataDirectory();
        return BootstrapAsync(appEnvironment);
    }

    public static Task<CoreConfigServicesBundle> BootstrapAsync(ConfigEnvironment configEnvironment)
    {
        return BootstrapAsync(configEnvironment, new JsonConfigStore(configEnvironment));
    }

    public static async Task<CoreConfigServicesBundle> BootstrapAsync(
        ConfigEnvironment configEnvironment,
        IConfigStore configStore)
    {
        var configLoader = new ConfigLoader(configStore);
        var appConfig = await configLoader.LoadOrCreateDefaultAsync();
        return new CoreConfigServicesBundle(configEnvironment, configStore, configLoader, appConfig);
    }
}
