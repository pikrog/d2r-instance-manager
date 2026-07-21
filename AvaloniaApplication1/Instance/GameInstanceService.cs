using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Account;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Region;

namespace AvaloniaApplication1.Instance;

public class GameInstanceService(ConfigService configService, GameInstanceManager gameInstanceManager)
{
    public event Action<Guid>? InstanceStateChanged
    {
        add => gameInstanceManager.InstanceStateChanged += value;
        remove => gameInstanceManager.InstanceStateChanged -= value;
    }

    private async Task AddAsync(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.AddInstance(snapshot));
        gameInstanceManager.Register(snapshot.Id);
    }

    private async Task UpdateAsync(GameInstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.UpdateInstance(snapshot));
    }

    public async Task SaveAsync(GameInstanceDraft draft)
    {
        // todo:
        // Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new GameInstanceSnapshot(
            id,
            draft.Name,
            draft.IsOnlineMode,
            draft.AccountId,
            draft.AuthenticationMethod,
            draft.RegionId,
            draft.Display,
            draft.IsNoSound,
            draft.IsWindowedMode,
            draft.RecallHotKey
        );

        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }
    
    public async Task RemoveAsync(Guid id)
    {
        await configService.ChangeAsync(context => context.RemoveInstance(id));
        gameInstanceManager.Remove(id);
    }

    public GameInstanceSnapshot GetInstanceConfigSnapshot(Guid id)
    {
        return configService.Config.GetInstance(id);
    }

    public RuntimeSnapshot GetInstanceRuntimeSnapshot(Guid id)
    {
        return gameInstanceManager.GetRuntimeSnapshot(id);
    }
    

    public GameInstanceSummary GetSummary(Guid id)
    {
        var configSnapshot = GetInstanceConfigSnapshot(id);
        var runtimeSnapshot = GetInstanceRuntimeSnapshot(id);
        var status = GameInstanceStatusMapper.Map(runtimeSnapshot);
        return new GameInstanceSummary(id, configSnapshot.Name, status, runtimeSnapshot.IsActive);
    }

    public IReadOnlyList<GameInstanceSummary> GetSummaries()
    {
        var instances = gameInstanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return configService.Config.GetAllInstances().Select(i =>
            {
                instances.TryGetValue(i.Id, out var runtimeSnapshot);
                var status = runtimeSnapshot is not null  
                    ? GameInstanceStatusMapper.Map(runtimeSnapshot) 
                    : GameInstanceStatus.Unknown; // todo: throw or ignore?
                var isActive = runtimeSnapshot?.IsActive ?? false; // ?
                return new GameInstanceSummary(i.Id, i.Name, status, isActive);
            }).ToList();
    }

    private string? ResolveDisplayId(DisplaySelection display)
    {
        var isFallbackAllowed = configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;
        return DisplayResolver.ResolveDisplayId(display, isFallbackAllowed);
    }

    public async Task LaunchAsync(Guid id)
    {
        var settings = configService.Config.GetGlobalSettings();
        
        var snapshot = GetInstanceConfigSnapshot(id);
        AuthenticationContext authenticationContext = new OfflineAuthenticationContext();
        if (snapshot.IsOnlineMode)
        {
            var account = configService.Config.GetAccount(snapshot.AccountId!.Value);
            var region = configService.Config.GetRegion(snapshot.RegionId!.Value);
            
            authenticationContext = snapshot.AuthenticationMethod switch
            {
                AuthenticationMethod.CommandLineArguments => new CliAuthenticationContext(account.Username, account.Password,
                    region.Address),
                AuthenticationMethod.OsiTokenRegistry => new OsiAuthenticationContext(region.Address),
                _ => throw new InvalidOperationException($"Unknown authentication method {snapshot.AuthenticationMethod}")
            };
        }

        var displayId = ResolveDisplayId(snapshot.Display);
        if (displayId is null)
            throw new InvalidOperationException("Failed to resolve display id"); // todo: return Result<Unit, Error>
        
        // todo: check if path is valid
        
        var instanceLaunchContext = new InstanceLaunchContext(
            settings.GameExecutablePath,
            authenticationContext,
            displayId,
            snapshot.IsNoSound,
            snapshot.IsWindowedMode
        );

        var multiboxUnlockRetryPolicy = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.UnlockMultiboxRetryDelayMs),
            settings.UnlockMultiboxMaxRetries
        );

        var gracefulInstanceStopRetryPolicy = new RetryPolicy(
            TimeSpan.FromMilliseconds(settings.GracefulInstanceCloseTimeoutMs),
            settings.GracefulInstanceCloseRetries
        );
        
        var forcefulInstanceStopTimeout = TimeSpan.FromMilliseconds(settings.ForcefulInstanceCloseTimeoutMs);
        
        var processStopPolicies = new ProcessStopPolicies(gracefulInstanceStopRetryPolicy, forcefulInstanceStopTimeout);
        
        var enginePolicies = new EnginePolicies(
            multiboxUnlockRetryPolicy,
            processStopPolicies
        );
        
        var engineLaunchContext = new EngineLaunchContext(instanceLaunchContext, enginePolicies);
        
        await gameInstanceManager.LaunchAsync(snapshot.Id, engineLaunchContext);
    }

    public async Task StopAsync(Guid id) => await gameInstanceManager.StopAsync(id);

    public void Show(Guid id) => gameInstanceManager.Show(id);
}