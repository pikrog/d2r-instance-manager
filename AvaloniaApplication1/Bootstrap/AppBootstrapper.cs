using System;
using System.Threading.Tasks;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class AppBootstrapper
{
    public static async Task<IServiceProvider> BootstrapAsync()
    {
        var coreConfigServicesBundle = await ConfigBootstrapper.BootstrapAsync();
        return BuildServiceProvider(coreConfigServicesBundle);
    }

    public static IServiceCollection CreateServiceCollection(CoreConfigServicesBundle coreConfigServicesBundle)
    {
        var services = new ServiceCollection();
        services.AddConfigServices(coreConfigServicesBundle);
        services.AddEngineServices();
        services.AddApplicationServices();
        return services;
    }

    public static ServiceProvider BuildServiceProvider(
        CoreConfigServicesBundle coreConfigServicesBundle,
        ServiceProviderOptions? options = null)
    {
        var services = CreateServiceCollection(coreConfigServicesBundle);
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
