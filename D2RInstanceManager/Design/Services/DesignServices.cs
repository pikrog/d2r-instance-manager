using System;
using Avalonia.Threading;
using D2RInstanceManager.Account;
using D2RInstanceManager.Bootstrap;
using D2RInstanceManager.Config;
using D2RInstanceManager.Config.Stores;
using D2RInstanceManager.Design.Config;
using D2RInstanceManager.Design.Providers.GameExecutablePath;
using D2RInstanceManager.Display;
using D2RInstanceManager.Engine;
using D2RInstanceManager.Engine.CommandLine;
using D2RInstanceManager.Engine.Coordination;
using D2RInstanceManager.Engine.Factories;
using D2RInstanceManager.GameExecutable;
using D2RInstanceManager.GameExecutable.Providers;
using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.HotKey.Config;
using D2RInstanceManager.Instance;
using D2RInstanceManager.Instance.ViewModels;
using D2RInstanceManager.Log;
using D2RInstanceManager.Log.Instance;
using D2RInstanceManager.Navigation;
using D2RInstanceManager.Notification;
using D2RInstanceManager.Overlay;
using D2RInstanceManager.Region;
using D2RInstanceManager.Secret;
using D2RInstanceManager.Toast;
using Microsoft.Extensions.Logging.Abstractions;

namespace D2RInstanceManager.Design.Services;

public static class DesignServices
{
    private static readonly AppConfig AppConfig = new AppDesignConfig();
    
    private static readonly ConfigContext ConfigContext = new(AppConfig);

    private static readonly IConfigStore ConfigStore = new DummyConfigStore();
    
    private static readonly ConfigService ConfigService = new(AppConfig, ConfigStore);

    public static readonly OverlayHost OverlayHost = new();
    
    public static readonly OverlayService OverlayService = new(OverlayHost);
    
    private static readonly FakeSecretProtectorFactory SecretProtectorFactory = new();

    private static readonly AccountSnapshotTransformer AccountSnapshotTransformer = new(SecretProtectorFactory);
    
    public static AccountService AccountService { get; } = new(ConfigService, AccountSnapshotTransformer);
    
    public static RegionService RegionService { get; } = new(ConfigService);

    public static DisplayService DisplayService { get; } = new(ConfigService);

    private static readonly MasterKeyService MasterKeyService = new();

    private static readonly MasterKeyDeriver MasterKeyDeriver = new();

    private static readonly SecretMigrationCoordinator SecretMigrationCoordinator = new(ConfigService, SecretProtectorFactory, MasterKeyDeriver, MasterKeyService);

    public static GlobalSettingsService GlobalSettingsService { get; } = new(ConfigService, SecretMigrationCoordinator);

    private static readonly IGameExecutablePathProvider GameExecutablePathProvider =
        new DesignGameExecutablePathProvider();

    public static GameExecutablePathLocator GameExecutablePathLocator { get; } =
        new([GameExecutablePathProvider]);
    
    private static readonly LaunchCoordinator LaunchCoordinator = new();

    private static readonly ShellExecuteArgumentFormatter ArgumentFormatter = new();

    private static readonly ArgumentStringBuilder ArgumentStringBuilder = new(ArgumentFormatter);
    
    private static readonly ProcessStartRequestFactory ProcessStartRequestFactory = new(ArgumentStringBuilder);
    
    private static readonly InstanceEngineFactory InstanceEngineFactory = 
        new(LaunchCoordinator, ProcessStartRequestFactory, NullLoggerFactory.Instance);
    
    private static readonly InstanceManager InstanceManager = new(InstanceEngineFactory);

    private static readonly InstanceNameRegistry InstanceNameRegistry = new();
    
    private static readonly InstanceRuntimeInitializer ManagerInitializer =
        new(ConfigContext, InstanceManager, InstanceNameRegistry);

    private static readonly InstanceConfigValidator InstanceConfigValidator = new(ConfigService);

    private static readonly GameExecutableFileValidator GameExecutableFileValidator = new();
    
    private static readonly GlobalSettingsPathValidator GlobalSettingsPathValidator = new(GameExecutableFileValidator);
    
    public static readonly GlobalSettingsValidator GlobalSettingsValidator = new(ConfigService, GlobalSettingsPathValidator);
    
    public static readonly PageHost PageHost = new();
    
    private static readonly InstanceLaunchContextFactory InstanceLaunchContextFactory = new(ConfigService,
        AccountSnapshotTransformer,
        GameExecutableFileValidator,
        NullLogger<InstanceLaunchContextFactory>.Instance);

    public static readonly NotificationCounter InstanceNotificationCounter = new();

    private static readonly InstanceActivityTracker InstanceActivityTracker = new();
    
    public static InstanceService InstanceService { get; } = 
        new InstanceDesignService(
            ConfigService,
            InstanceConfigValidator,
            InstanceManager, 
            InstanceNameRegistry,
            InstanceLaunchContextFactory,
            InstanceActivityTracker,
            InstanceNotificationCounter,
            NullLogger<InstanceService>.Instance);

    public static readonly IInstancePresenter InstancePresenter = new InstanceDesignPresenter(InstanceService);
    
    private static readonly PageDesignProvider PageProvider = new();
    
    public static readonly NavigationService NavigationService = new(PageHost, PageProvider);
    
    public static readonly FakeToastHost ToastHost = new();

    public static readonly ToastService ToastService = new(ToastHost);

    private static readonly IHotKeyConfigValidator HotKeyConfigValidator = new FakeHotKeyConfigValidator();

    public static readonly EditInstanceFormViewModelFactory EditInstanceFormViewModelFactory =
        new(HotKeyConfigValidator, AccountService, RegionService, DisplayService);
    
    public static readonly ILogStoreReader LogStore = new FakeLogStore();
    
    public static readonly NotificationCounter LogNotificationCounter = new(2);
    
    public static readonly LogService LogService = new();

    public static readonly FakeDisplayConfigMonitor DisplayConfigMonitor = new();

    private static readonly DisplayConfigChangeNotifier DisplayConfigChangeNotifier =
        new(DisplayConfigMonitor, ToastService);
    
    static DesignServices()
    {
        ManagerInitializer.Initialize();
        
        DisplayConfigChangeNotifier.Initialize();

        DispatcherTimer.RunOnce(
            () => DisplayConfigMonitor.TriggerDisplayConfigChanged(), 
            TimeSpan.FromSeconds(2.5));
    }
}