using System.Threading.Tasks;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Bootstrap;

public static class ConfigBootstrapper
{
    public static Task<CoreConfigServicesBundle> BootstrapAsync()
    {
        var appEnvironment = AppEnvironment.CreateInApplicationDataDirectory();
        return BootstrapAsync(appEnvironment);
    }

    public static Task<CoreConfigServicesBundle> BootstrapAsync(AppEnvironment appEnvironment)
    {
        return BootstrapAsync(appEnvironment, new JsonConfigStore(appEnvironment));
    }

    public static async Task<CoreConfigServicesBundle> BootstrapAsync(
        AppEnvironment appEnvironment,
        IConfigStore configStore)
    {
        var configLoader = new ConfigLoader(configStore);
        var appConfig = await configLoader.LoadOrCreateDefaultAsync();
        return new CoreConfigServicesBundle(appEnvironment, configStore, configLoader, appConfig);
    }
}
