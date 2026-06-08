using System.Threading.Tasks;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Bootstrap;

public static class ConfigBootstrapper
{
    public static Task<CoreConfigServicesBundle> Bootstrap()
    {
        var appEnvironment = AppEnvironment.CreateInApplicationDataDirectory();
        return Bootstrap(appEnvironment);
    }

    public static Task<CoreConfigServicesBundle> Bootstrap(AppEnvironment appEnvironment)
    {
        return Bootstrap(appEnvironment, new JsonConfigStore(appEnvironment));
    }

    public static async Task<CoreConfigServicesBundle> Bootstrap(
        AppEnvironment appEnvironment,
        IConfigStore configStore)
    {
        var configLoader = new ConfigLoader(configStore);
        var appConfig = await configLoader.LoadOrCreateDefaultAsync();
        return new CoreConfigServicesBundle(appEnvironment, configStore, configLoader, appConfig);
    }
}
