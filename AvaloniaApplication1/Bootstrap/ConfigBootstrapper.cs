using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Config.Stores;
using Microsoft.Extensions.Logging;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public static class ConfigBootstrapper
{
    public static Task<CoreConfigServicesBundle> BootstrapAsync(
        AppDataEnvironment appDataEnvironment,
        Serilog.ILogger logger
        )
    {
        var configEnvironment = ConfigEnvironment.Derive(appDataEnvironment);
        var loggerFactory = LoggerFactory.Create(builder => builder.AddSerilog(logger, dispose: false));
        
        return BootstrapAsync(configEnvironment, loggerFactory);
    }

    public static Task<CoreConfigServicesBundle> BootstrapAsync(
        ConfigEnvironment configEnvironment, 
        ILoggerFactory loggerFactory)
    {
        var configStoreLogger = loggerFactory.CreateLogger<JsonConfigStore>();
        var configStore = new JsonConfigStore(configEnvironment, configStoreLogger);
        
        return BootstrapAsync(configEnvironment, configStore, loggerFactory);
    }

    public static async Task<CoreConfigServicesBundle> BootstrapAsync(
        ConfigEnvironment configEnvironment,
        IConfigStore configStore,
        ILoggerFactory loggerFactory)
    {
        var configLoaderLogger = loggerFactory.CreateLogger<ConfigLoader>();
        var configLoader = new ConfigLoader(configStore, configLoaderLogger);
        
        var appConfig = await configLoader.LoadOrCreateDefaultAsync();
        
        return new CoreConfigServicesBundle(configEnvironment, configStore, configLoader, appConfig);
    }
}
