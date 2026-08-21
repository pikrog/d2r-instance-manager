using AvaloniaApplication1.Account;
using AvaloniaApplication1.Account.ViewModels;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GameExecutable.Providers;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.MainWindow;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Region;
using AvaloniaApplication1.Region.ViewModels;
using Microsoft.Extensions.DependencyInjection;

namespace AvaloniaApplication1.Bootstrap;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddConfigServices(CoreConfigServicesBundle coreConfigServicesBundle)
        {
            services.AddSingleton(coreConfigServicesBundle.ConfigEnvironment);
            services.AddSingleton(coreConfigServicesBundle.ConfigStore);
            services.AddSingleton(coreConfigServicesBundle.ConfigLoader);
            services.AddSingleton(coreConfigServicesBundle.AppConfig);
            services.AddSingleton<ConfigContext>();
            services.AddSingleton<ConfigService>();
        }

        public void AddEngineServices()
        {
            services.AddSingleton<LaunchCoordinator>();
            services.AddSingleton<IArgumentFormatter, ShellExecuteArgumentFormatter>();
            services.AddSingleton<ArgumentListFormatter>();
            services.AddSingleton<IArgumentStringBuilder, ArgumentStringBuilder>();
            services.AddSingleton<ArgumentsFactory>();
            services.AddSingleton<ProcessStartInfoFactory>();
            services.AddSingleton<InstanceEngineFactory>();
            services.AddSingleton<InstanceManager>();
        }

        public void AddApplicationServices()
        {
            services.AddSingleton<InstanceConfigValidator>();
            services.AddSingleton<GlobalSettingsValidator>();
            
            services.AddSingleton<AccountService>();
            services.AddSingleton<RegionService>();
            services.AddSingleton<InstanceService>();
            services.AddSingleton<GlobalSettingsService>();
            services.AddSingleton<DisplayService>();
            
            services.AddSingleton<IGameExecutablePathProvider, GameConfigStorePathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, UninstallEntryPathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, ProgramFilesPathProvider>();
            services.AddSingleton<GameExecutablePathLocator>();
            
            services.AddSingleton<OverlayHost>();
            services.AddSingleton<IOverlayHost>(sp => sp.GetRequiredService<OverlayHost>());
            services.AddSingleton<IOverlayController>(sp => sp.GetRequiredService<OverlayHost>());
            services.AddSingleton<OverlayService>();

            services.AddTransient<InstancesPageViewModel>();
            services.AddTransient<AccountsPageViewModel>();
            services.AddTransient<RegionsPageViewModel>();
            services.AddTransient<GlobalSettingsPageViewModel>();
            
            services.AddSingleton<PageHost>();
            services.AddSingleton<IPageHost>(sp => sp.GetRequiredService<PageHost>());
            services.AddSingleton<IPageController>(sp => sp.GetRequiredService<PageHost>());
            services.AddSingleton<IPageProvider, PageProvider>();
            services.AddSingleton<NavigationService>();

            services.AddSingleton<HotKeyService>();
            services.AddSingleton<IHotKeyService>(sp => sp.GetRequiredService<HotKeyService>());
            services.AddSingleton<IHotKeyWindowInitializer>(sp => sp.GetRequiredService<HotKeyService>());
            services.AddSingleton<HotKeyManager>();
            
            services.AddSingleton<InstanceHotKeyManager>();
            
            services.AddTransient<MainWindowViewModel>();

            services.AddSingleton<InstanceManagerBootstrapper>();
            services.AddSingleton<AppRuntimeBootstrapper>();
            
            services.AddSingleton<MainWindowRuntimeBootstrapper>();
        }
    }
}