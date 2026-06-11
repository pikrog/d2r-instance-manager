using System;
using Avalonia.Input;
using AvaloniaApplication1.Bootstrap;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Design.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Engine.Providers;
using AvaloniaApplication1.Models;
using AvaloniaApplication1.Services;
using AvaloniaApplication1.Snapshots;

namespace AvaloniaApplication1.Design.Services;

public static class DesignServices
{
    private static readonly AppConfig AppConfig = new AppDesignConfig();
    
    private static readonly ConfigContext ConfigContext = new(AppConfig);

    private static readonly IConfigStore ConfigStore = new DummyConfigStore();
    
    private static readonly ConfigService ConfigService = new(ConfigContext, ConfigStore);
    
    public static AccountService AccountService { get; } = new(ConfigService);
    
    public static RegionService RegionService { get; } = new(ConfigService);
    
    private static readonly LaunchCoordinator LaunchCoordinator = new();

    private static readonly ShellExecuteArgumentFormatter ArgumentFormatter = new();

    private static readonly ArgumentStringBuilder ArgumentStringBuilder = new(ArgumentFormatter);

    private static readonly ArgumentsFactory ArgumentsFactory = new(ArgumentStringBuilder);

    private static readonly DisplayResolverSettingsProvider DisplayResolverSettingsProvider = new(ConfigService);
    
    private static readonly DisplayResolver DisplayResolver = new(DisplayResolverSettingsProvider);
    
    private static readonly ProcessStartInfoFactory ProcessStartInfoFactory = new(ArgumentsFactory, DisplayResolver);

    private static readonly GameInstanceEngineFactory GameInstanceEngineFactory = new(LaunchCoordinator, ProcessStartInfoFactory);
    
    private static readonly GameInstanceManager GameInstanceManager = new(GameInstanceEngineFactory);

    private static readonly GameInstanceManagerBootstrapper ManagerBootstrapper =
        new(ConfigContext, GameInstanceManager);

    public static GameInstanceService GameInstanceService { get; } =
        new(ConfigService, GameInstanceManager, AccountService, RegionService);
    
    static DesignServices()
    {
        ManagerBootstrapper.Bootstrap();
    }
}