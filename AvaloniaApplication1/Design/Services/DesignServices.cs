using System;
using Avalonia.Input;
using AvaloniaApplication1.Bootstrap;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Design.Config;
using AvaloniaApplication1.Engine;
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

    private static readonly GameInstanceManager GameInstanceManager = new();

    private static readonly GameInstanceManagerBootstrapper ManagerBootstrapper =
        new(ConfigContext, GameInstanceManager);

    public static GameInstanceService GameInstanceService { get; } =
        new(ConfigService, GameInstanceManager, AccountService, RegionService);
    
    static DesignServices()
    {
        ManagerBootstrapper.Bootstrap();
    }
}