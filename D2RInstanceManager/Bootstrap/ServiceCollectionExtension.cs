using System;
using D2RInstanceManager.Account;
using D2RInstanceManager.Account.ViewModels;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.Engine;
using D2RInstanceManager.Engine.CommandLine;
using D2RInstanceManager.Engine.Coordination;
using D2RInstanceManager.Engine.Factories;
using D2RInstanceManager.GameExecutable;
using D2RInstanceManager.GameExecutable.Providers;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.HotKey.Config;
using D2RInstanceManager.HotKey.Platform;
using D2RInstanceManager.Initialization;
using D2RInstanceManager.Instance;
using D2RInstanceManager.Instance.ViewModels;
using D2RInstanceManager.Log;
using D2RInstanceManager.MainWindow;
using D2RInstanceManager.Navigation;
using D2RInstanceManager.Notification;
using D2RInstanceManager.Overlay;
using D2RInstanceManager.Region;
using D2RInstanceManager.Region.ViewModels;
using D2RInstanceManager.Secret;
using D2RInstanceManager.Toast;
using D2RInstanceManager.Unlock;
using Microsoft.Extensions.DependencyInjection;
using Serilog;

namespace D2RInstanceManager.Bootstrap;

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
            services.AddSingleton<IGlobalSettingsPathValidator, GlobalSettingsPathValidator>();
            services.AddSingleton<IGlobalSettingsValidator, GlobalSettingsValidator>();
            
            services.AddSingleton<AccountService>();
            services.AddSingleton<RegionService>();
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
            
            services.AddSingleton<InstanceService>();
            services.AddSingleton<IInstanceConfigProvider>(sp => sp.GetRequiredService<InstanceService>());
            services.AddSingleton<InstanceConfigValidator>();
            services.AddSingleton<IInstancePresenter, InstancePresenter>();
            services.AddSingleton<InstanceLaunchContextFactory>();
            services.AddSingleton<InstanceActivityTracker>();
            services.AddSingleton<InstanceExitReporter>();
            services.AddNotificationCounter(NotificationCounterKey.Instances);
            services.AddSingleton<InstanceHotKeyBindingCoordinator>();
            services.AddSingleton<InstanceHotKeyConfigProvider>();
            services.AddSingleton<IHotKeyConfigProvider, InstanceHotKeyConfigProvider>(
                sp => sp.GetRequiredService<InstanceHotKeyConfigProvider>());
            services.AddTransient<InstancesPageViewModel>();
            services.AddSingleton<EditInstanceFormViewModelFactory>();
            
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
            
            services.AddTransient<AccountsPageViewModel>();
            services.AddTransient<RegionsPageViewModel>();
            services.AddTransient<LogsPageViewModel>();
            services.AddTransient<GlobalSettingsPageViewModel>();
            
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
