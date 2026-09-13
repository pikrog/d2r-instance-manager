using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class AppBootstrapper
{
    public static Task<ServiceProvider> BootstrapAsync()
    {
        var appDataEnvironment = AppDataEnvironment.CreateDefault();
        return BootstrapAsync(appDataEnvironment);
    }
    
    public static async Task<ServiceProvider> BootstrapAsync(AppDataEnvironment appDataEnvironment)
    {
        var coreLoggingServicesBundle = LoggingBootstrapper.Bootstrap(appDataEnvironment);
        
        var coreConfigServicesBundle = 
            await ConfigBootstrapper.BootstrapAsync(appDataEnvironment, coreLoggingServicesBundle.Logger);
        
        return BuildServiceProvider(coreLoggingServicesBundle, coreConfigServicesBundle);   
    }

    public static IServiceCollection CreateServiceCollection(
        CoreLoggingServicesBundle coreLoggingServicesBundle,
        CoreConfigServicesBundle coreConfigServicesBundle)
    {
        var services = new ServiceCollection();
        services.AddLoggingServices(coreLoggingServicesBundle);
        services.AddConfigServices(coreConfigServicesBundle);
        services.AddEngineServices();
        services.AddApplicationServices();
        return services;
    }

    public static ServiceProvider BuildServiceProvider(
        CoreLoggingServicesBundle coreLoggingServicesBundle,
        CoreConfigServicesBundle coreConfigServicesBundle,
        ServiceProviderOptions? options = null)
    {
        var services = CreateServiceCollection(coreLoggingServicesBundle, coreConfigServicesBundle);
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
