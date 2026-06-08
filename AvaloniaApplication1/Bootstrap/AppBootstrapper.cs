using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class AppBootstrapper
{
    public static async Task<IServiceProvider> BootstrapAsync()
    {
        var coreConfigServicesBundle = await ConfigBootstrapper.Bootstrap();
        var provider = BuildServiceProvider(coreConfigServicesBundle);
        BootstrapRuntime(provider);
        return provider;
    }

    public static IServiceCollection CreateServiceCollection(CoreConfigServicesBundle coreConfigServicesBundle)
    {
        var services = new ServiceCollection();
        services.AddConfigServices(coreConfigServicesBundle);
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

    public static void BootstrapRuntime(IServiceProvider provider)
    {
        provider.GetRequiredService<GameInstanceManagerBootstrapper>().Bootstrap();
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
