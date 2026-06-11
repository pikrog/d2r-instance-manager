using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Providers;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddConfigServices(CoreConfigServicesBundle coreConfigServicesBundle)
        {
            services.AddSingleton(coreConfigServicesBundle.AppEnvironment);
            services.AddSingleton(coreConfigServicesBundle.ConfigStore);
            services.AddSingleton(coreConfigServicesBundle.ConfigLoader);
            services.AddSingleton(coreConfigServicesBundle.AppConfig);
            services.AddSingleton<ConfigContext>();
            services.AddSingleton<ConfigService>();
        }

        public void AddEngineServices()
        {
            services.AddSingleton<LaunchCoordinator>();
            services.AddSingleton<ShellExecuteArgumentFormatter>();
            services.AddSingleton<ArgumentStringBuilder>();
            services.AddSingleton<ArgumentsFactory>();
            services.AddSingleton<DisplayResolverSettingsProvider>();
            services.AddSingleton<DisplayResolver>();
            services.AddSingleton<ProcessStartInfoFactory>();
            services.AddSingleton<GameInstanceEngineFactory>();
            services.AddSingleton<GameInstanceManager>();
        }

        public void AddApplicationServices()
        {
            services.AddSingleton<GameInstanceManagerBootstrapper>();

            services.AddSingleton<AccountService>();
            services.AddSingleton<RegionService>();
            services.AddSingleton<GameInstanceService>();
            services.AddSingleton<DialogService>();

            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<RegionsPageViewModel>();
            services.AddTransient<AccountsPageViewModel>();
            services.AddTransient<InstancesPageViewModel>();
        }
    }
}