using AvaloniaApplication1.Account;
using AvaloniaApplication1.Bootstrap;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Config.Stores;
using AvaloniaApplication1.Design.Config;
using AvaloniaApplication1.Design.Providers.GameExecutablePath;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GameExecutable.Providers;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Navigation;
using AvaloniaApplication1.Overlay;
using AvaloniaApplication1.Region;

namespace AvaloniaApplication1.Design.Services;

public static class DesignServices
{
    private static readonly AppConfig AppConfig = new AppDesignConfig();
    
    private static readonly ConfigContext ConfigContext = new(AppConfig);

    private static readonly IConfigStore ConfigStore = new DummyConfigStore();
    
    private static readonly ConfigService ConfigService = new(ConfigContext, ConfigStore);

    public static readonly OverlayHost OverlayHost = new();
    
    public static readonly OverlayService OverlayService = new(OverlayHost);
    
    public static AccountService AccountService { get; } = new(ConfigService);
    
    public static RegionService RegionService { get; } = new(ConfigService);

    public static DisplayService DisplayService { get; } = new(ConfigService);

    public static GlobalSettingsService GlobalSettingsService { get; } = new(ConfigService);

    private static readonly IGameExecutablePathProvider GameExecutablePathProvider =
        new DesignGameExecutablePathProvider();

    public static GameExecutablePathLocator GameExecutablePathLocator { get; } = new([GameExecutablePathProvider]);
    
    private static readonly LaunchCoordinator LaunchCoordinator = new();

    private static readonly ShellExecuteArgumentFormatter ArgumentFormatter = new();

    private static readonly ArgumentStringBuilder ArgumentStringBuilder = new(ArgumentFormatter);

    private static readonly ArgumentsFactory ArgumentsFactory = new(ArgumentStringBuilder);
    
    private static readonly ProcessStartInfoFactory ProcessStartInfoFactory = new(ArgumentsFactory);

    private static readonly InstanceEngineFactory InstanceEngineFactory = new(LaunchCoordinator, ProcessStartInfoFactory);
    
    private static readonly InstanceManager InstanceManager = new(InstanceEngineFactory);

    private static readonly InstanceManagerBootstrapper ManagerBootstrapper =
        new(ConfigContext, InstanceManager);

    private static readonly InstanceConfigValidator InstanceConfigValidator = new(ConfigService);

    public static readonly GlobalSettingsValidator GlobalSettingsValidator = new(ConfigService);
    
    public static readonly PageHost PageHost = new();
    
    public static InstanceService InstanceService { get; } = new InstanceDesignService(ConfigService, InstanceConfigValidator, InstanceManager);
    
    private static readonly PageDesignProvider PageProvider = new();
    
    public static readonly NavigationService NavigationService = new(PageHost, PageProvider);
    
    static DesignServices()
    {
        ManagerBootstrapper.Bootstrap();
    }
}