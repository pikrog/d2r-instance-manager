using System;
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
using AvaloniaApplication1.HotKey.Config;
using AvaloniaApplication1.HotKey.Platform;
using AvaloniaApplication1.Initialization;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Log;
using AvaloniaApplication1.MainWindow;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Notification;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Region;
using AvaloniaApplication1.Region.ViewModels;
using AvaloniaApplication1.Secret;
using AvaloniaApplication1.Toast;
using AvaloniaApplication1.Unlock;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public static class ServiceCollectionExtension
{
    extension(IServiceCollection services)
    {
        public void AddLoggingServices(CoreLoggingServicesBundle bundle)
        {
            services.AddSingleton<ILogStoreReader>(bundle.LogStore);
            services.AddSingleton<ILogStoreWriter>(bundle.LogStore);
            services.AddSingleton(bundle.LogSettings);
            services.AddSingleton(bundle.InstanceNameRegistry);
            services.AddSingleton<ILogService, LogService>();
            services.AddNotificationCounter(NotificationCounterKey.Logs, bundle.NotificationCounter);
            services.AddLogging(builder => builder.AddSerilog(bundle.Logger, dispose: true));
        }
        
        public void AddConfigServices(CoreConfigServicesBundle bundle)
        {
            services.AddSingleton(bundle.ConfigEnvironment);
            services.AddSingleton(bundle.ConfigStore);
            services.AddSingleton(bundle.ConfigLoader);
            services.AddSingleton(bundle.AppConfig);
            services.AddSingleton<ConfigContext>();
            services.AddSingleton<ConfigService>();
        }

        public void AddEngineServices()
        {
            services.AddSingleton<LaunchCoordinator>();
            services.AddSingleton<IArgumentFormatter, ShellExecuteArgumentFormatter>();
            services.AddSingleton<IArgumentStringBuilder, ArgumentStringBuilder>();
            services.AddSingleton<ProcessStartRequestFactory>();
            services.AddSingleton<InstanceEngineFactory>();
            services.AddSingleton<InstanceManager>();
        }

        private static NotificationCounter GetNotificationCounter(IServiceProvider serviceProvider, object? key) =>
            serviceProvider.GetRequiredKeyedService<NotificationCounter>(key);
        
        private void AddNotificationCounter(NotificationCounterKey key, NotificationCounter? counter = null)
        {
            if (counter is null)
                services.AddKeyedSingleton<NotificationCounter>(key);
            else
                services.AddKeyedSingleton(key, counter);
            
            services.AddKeyedSingleton<INotificationAcknowledger>(key, GetNotificationCounter);
            services.AddKeyedSingleton<INotificationSink>(key, GetNotificationCounter);
            services.AddKeyedSingleton<INotificationSource>(key, GetNotificationCounter);
        }
        
        private void AddRegisteredRuntimeInitializer<T>() where T : IRuntimeInitializer =>
            services.AddSingleton<IRuntimeInitializer>(sp => sp.GetRequiredService<T>());
        
        private void AddRegisteredWindowRuntimeInitializer<T>() where T : IWindowRuntimeInitializer =>
            services.AddSingleton<IWindowRuntimeInitializer>(sp => sp.GetRequiredService<T>());
        
        public void AddApplicationServices()
        {
            services.AddSingleton<InstanceConfigValidator>();
            services.AddSingleton<IGlobalSettingsPathValidator, GlobalSettingsPathValidator>();
            services.AddSingleton<IGlobalSettingsValidator, GlobalSettingsValidator>();
            
            services.AddSingleton<AccountService>();
            services.AddSingleton<RegionService>();
            services.AddSingleton<InstanceService>();
            services.AddSingleton<IInstanceConfigProvider>(sp => sp.GetRequiredService<InstanceService>());
            services.AddSingleton<GlobalSettingsService>();
            services.AddSingleton<DisplayService>();
            
            services.AddSingleton<IMasterKeyDeriver, MasterKeyDeriver>();
            services.AddSingleton<MasterKeyService>();
            services.AddSingleton<IMasterKeyProvider>(sp => sp.GetRequiredService<MasterKeyService>());
            services.AddSingleton<ISecretMigrationCoordinator, SecretMigrationCoordinator>();
            services.AddSingleton<INonKeyBasedSecretProtectionVerifier, SecretProtectionVerifier>();
            services.AddSingleton<ISecretProtectorFactory, SecretProtectorFactory>();
            services.AddSingleton<SecretMigrationCoordinator>();
            
            services.AddSingleton<AccountSnapshotTransformer>();
            
            services.AddSingleton<InstanceLaunchContextFactory>();

            services.AddNotificationCounter(NotificationCounterKey.Instances);

            services.AddSingleton<IInstancePresenter, InstancePresenter>();
            
            services.AddSingleton<IGameExecutablePathProvider, GameConfigStorePathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, UninstallEntryPathProvider>();
            services.AddSingleton<IGameExecutablePathProvider, ProgramFilesPathProvider>();
            services.AddSingleton<IGameExecutablePathLocator, GameExecutablePathLocator>();
            services.AddSingleton<IGameExecutableFileValidator, GameExecutableFileValidator>();
            
            services.AddSingleton<OverlayHost>();
            services.AddSingleton<IOverlayHost>(sp => sp.GetRequiredService<OverlayHost>());
            services.AddSingleton<IOverlayController>(sp => sp.GetRequiredService<OverlayHost>());
            services.AddSingleton<OverlayService>();

            services.AddSingleton<ToastHost>();
            services.AddSingleton<IToastHost>(sp => sp.GetRequiredService<ToastHost>());
            services.AddSingleton<IToastController>(sp => sp.GetRequiredService<ToastHost>());
            services.AddSingleton<ToastService>();

            services.AddSingleton<DisplayConfigMonitor>();
            services.AddSingleton<IDisplayConfigMonitor>(sp => sp.GetRequiredService<DisplayConfigMonitor>());
            services.AddSingleton<DisplayConfigChangeNotifier>();

            services.AddTransient<InstancesPageViewModel>();
            services.AddTransient<AccountsPageViewModel>();
            services.AddTransient<RegionsPageViewModel>();
            services.AddTransient<LogsPageViewModel>();
            services.AddTransient<GlobalSettingsPageViewModel>();
            
            services.AddSingleton<EditInstanceFormViewModelFactory>();
            
            services.AddSingleton<PageHost>();
            services.AddSingleton<IPageHost>(sp => sp.GetRequiredService<PageHost>());
            services.AddSingleton<IPageController>(sp => sp.GetRequiredService<PageHost>());
            services.AddSingleton<IPageProvider, PageProvider>();
            services.AddSingleton<NavigationService>();

            services.AddSingleton<HotKeyService>();
            services.AddSingleton<IHotKeyService>(sp => sp.GetRequiredService<HotKeyService>());
            services.AddSingleton<HotKeyRegistrationManager>();
            services.AddSingleton<IHotKeySuspensionCoordinator>(sp => sp.GetRequiredService<HotKeyRegistrationManager>());
            services.AddSingleton<IHotKeyConfigValidator, HotKeyConfigValidator>();
            
            services.AddSingleton<InstanceHotKeyBindingCoordinator>();
            services.AddSingleton<InstanceHotKeyConfigProvider>();
            services.AddSingleton<IHotKeyConfigProvider, InstanceHotKeyConfigProvider>(sp => sp.GetRequiredService<InstanceHotKeyConfigProvider>());

            services.AddSingleton<IUnlockService, UnlockService>();
            services.AddSingleton<UnlockCoordinator>();
            services.AddTransient<UnlockWindowViewModel>();
            
            services.AddTransient<MainWindowViewModel>();

            services.AddSingleton<IRuntimeInitializer, InstanceRuntimeInitializer>();
            services.AddRegisteredRuntimeInitializer<InstanceHotKeyBindingCoordinator>();
            services.AddRegisteredRuntimeInitializer<DisplayConfigChangeNotifier>();
            services.AddSingleton<RuntimeBootstrapper>();
            
            services.AddRegisteredWindowRuntimeInitializer<HotKeyService>();
            services.AddRegisteredWindowRuntimeInitializer<DisplayConfigMonitor>();
            services.AddSingleton<WindowRuntimeBootstrapper>();
        }
    }
}
