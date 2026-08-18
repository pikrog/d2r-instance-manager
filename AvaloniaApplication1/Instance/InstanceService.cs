using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Authentication.Models;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.Instance.Models;

namespace AvaloniaApplication1.Instance;

public class InstanceService(
    ConfigService configService,
    InstanceConfigValidator validator,
    InstanceManager instanceManager)
{
    public event Action<Guid>? InstanceStateChanged
    {
        add => instanceManager.InstanceStateChanged += value;
        remove => instanceManager.InstanceStateChanged -= value;
    }

    private async Task AddAsync(InstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.AddInstance(snapshot));
        instanceManager.Register(snapshot.Id);
    }

    private async Task UpdateAsync(InstanceSnapshot snapshot)
    {
        await configService.ChangeAsync(context => context.UpdateInstance(snapshot));
    }

    public async Task SaveAsync(InstanceDraft draft)
    {
        // todo:
        // Validate(draft);

        var id = draft.Id ?? Guid.NewGuid();
        var snapshot = new InstanceSnapshot(
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
        instanceManager.Remove(id);
    }

    public InstanceSnapshot GetConfigSnapshot(Guid id)
    {
        return configService.Config.GetInstance(id);
    }

    public RuntimeSnapshot GetRuntimeSnapshot(Guid id)
    {
        return instanceManager.GetRuntimeSnapshot(id);
    }

    private InstanceSummary CreateSummary(InstanceSnapshot snapshot, RuntimeSnapshot runtimeSnapshot)
    {
        var status = InstanceStatusMapper.Map(runtimeSnapshot);
        var issues = validator.Validate(snapshot);
        return new InstanceSummary(snapshot.Id, snapshot.Name, status, runtimeSnapshot.IsActive, issues);
    }

    public InstanceSummary GetSummary(Guid id)
    {
        var configSnapshot = GetConfigSnapshot(id);
        var runtimeSnapshot = GetRuntimeSnapshot(id);
        return CreateSummary(configSnapshot, runtimeSnapshot);
    }

    public virtual IReadOnlyList<InstanceSummary> GetSummaries()
    {
        var instances = instanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return configService.Config.GetAllInstances().Select(i =>
            {
                var runtimeSnapshot = instances[i.Id];
                return CreateSummary(i, runtimeSnapshot);
            }).ToList();
    }

    public Task<int> GetActiveCountAsync() => instanceManager.GetActiveCountAsync();

    private string? ResolveDisplayId(DisplaySelection display)
    {
        var isFallbackAllowed = configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;
        return DisplayResolver.ResolveDisplayId(display, isFallbackAllowed);
    }

    public async Task LaunchAsync(Guid id)
    {
        var settings = configService.Config.GetGlobalSettings();
        
        var snapshot = GetConfigSnapshot(id);
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
        
        await instanceManager.LaunchAsync(snapshot.Id, engineLaunchContext);
    }

    public async Task StopAsync(Guid id) => await instanceManager.StopAsync(id);

    public Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync() => 
        instanceManager.RequestGracefulShutdownAllAsync();

    public void Show(Guid id) => instanceManager.Show(id);
}
