using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Providers.GameExecutablePath;
using AvaloniaApplication1.Services;
using Microsoft.Extensions.DependencyInjection;
using AccountsPageViewModel = AvaloniaApplication1.ViewModels.Account.AccountsPageViewModel;
using GlobalSettingsPageViewModel = AvaloniaApplication1.ViewModels.GlobalSettings.GlobalSettingsPageViewModel;
using InstancesPageViewModel = AvaloniaApplication1.ViewModels.Instance.InstancesPageViewModel;
using MainWindowViewModel = AvaloniaApplication1.ViewModels.MainWindow.MainWindowViewModel;
using RegionsPageViewModel = AvaloniaApplication1.ViewModels.Region.RegionsPageViewModel;

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
            services.AddSingleton<IArgumentFormatter, ShellExecuteArgumentFormatter>();
            services.AddSingleton<ArgumentListFormatter>();
            services.AddSingleton<IArgumentStringBuilder, ArgumentStringBuilder>();
            services.AddSingleton<ArgumentsFactory>();
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
            services.AddSingleton<GlobalSettingsService>();
            services.AddSingleton<DisplayService>();
            
            services.AddSingleton<IGameExecutablePathProvider, GameConfigStorePathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, UninstallEntryPathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, ProgramFilesPathProvider>();
            services.AddSingleton<GameExecutablePathLocator>();
            
            services.AddTransient<MainWindowViewModel>();
            services.AddTransient<RegionsPageViewModel>();
            services.AddTransient<AccountsPageViewModel>();
            services.AddTransient<InstancesPageViewModel>();
            services.AddTransient<GlobalSettingsPageViewModel>();
        }
    }
}