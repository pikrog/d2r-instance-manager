using System.Threading.Tasks;
using AvaloniaApplication1.CommandLine;
using AvaloniaApplication1.Log;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class AppBootstrapper
{
    public static Task<ServiceProvider> BootstrapAsync(CliOptions cliOptions)
    {
        var appDataEnvironment = AppDataEnvironment.CreateDefault();
        return BootstrapAsync(appDataEnvironment, cliOptions);
    }
    
    public static async Task<ServiceProvider> BootstrapAsync(AppDataEnvironment appDataEnvironment, CliOptions cliOptions)
    {
        var logLevel = LogLevelMapper.Map(cliOptions.CliLogLevel);
        var logSettings = new LogSettings()
        {
            MinimumLevel = logLevel,
        };
        
        var coreLoggingServicesBundle = LoggingBootstrapper.Bootstrap(appDataEnvironment, logSettings);
        
        var coreConfigServicesBundle = 
            await ConfigBootstrapper.BootstrapAsync(appDataEnvironment, coreLoggingServicesBundle.Logger);
        
        return BuildServiceProvider(coreLoggingServicesBundle, coreConfigServicesBundle, cliOptions);   
    }

    public static IServiceCollection CreateServiceCollection(
        CoreLoggingServicesBundle coreLoggingServicesBundle,
        CoreConfigServicesBundle coreConfigServicesBundle,
        CliOptions cliOptions)
    {
        var services = new ServiceCollection();
        services.AddLoggingServices(coreLoggingServicesBundle);
        services.AddConfigServices(coreConfigServicesBundle);
        services.AddSingleton(cliOptions);
        services.AddEngineServices();
        services.AddApplicationServices();
        return services;
    }

    public static ServiceProvider BuildServiceProvider(
        CoreLoggingServicesBundle coreLoggingServicesBundle,
        CoreConfigServicesBundle coreConfigServicesBundle,
        CliOptions cliOptions,
        ServiceProviderOptions? options = null)
    {
        var services = CreateServiceCollection(coreLoggingServicesBundle, coreConfigServicesBundle, cliOptions);
        return services.BuildServiceProvider(options ?? CreateDefaultServiceProviderOptions());
    }

    private static ServiceProviderOptions CreateDefaultServiceProviderOptions()
    {
        return new ServiceProviderOptions
        {
            ValidateOnBuild = true,
            ValidateScopes = true
        };
    }
}
