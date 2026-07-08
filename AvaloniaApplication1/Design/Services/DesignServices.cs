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

    private static readonly GameInstanceEngineFactory GameInstanceEngineFactory = new(LaunchCoordinator, ProcessStartInfoFactory);
    
    private static readonly GameInstanceManager GameInstanceManager = new(GameInstanceEngineFactory);

    private static readonly GameInstanceManagerBootstrapper ManagerBootstrapper =
        new(ConfigContext, GameInstanceManager);
    
    public static GameInstanceService GameInstanceService { get; } =
        new(ConfigService, GameInstanceManager, AccountService, RegionService, DisplayService);
    
    static DesignServices()
    {
        ManagerBootstrapper.Bootstrap();
    }
}