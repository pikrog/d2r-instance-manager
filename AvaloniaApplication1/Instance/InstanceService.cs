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
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Instance;

public class InstanceService
{
    private readonly ConfigService _configService;
    
    private readonly InstanceConfigValidator _validator;
    
    private readonly InstanceManager _instanceManager;

    public event EventHandler<InstanceStateChangedEventArgs>? InstanceStateChanged;
    
    public event EventHandler<InstanceConfigChangedEventArgs>? InstanceConfigChanged;

    public InstanceService(ConfigService configService,
        InstanceConfigValidator validator,
        InstanceManager instanceManager)
    {
        _configService = configService;
        _validator = validator;
        _instanceManager = instanceManager;

        _instanceManager.InstanceStateChanged += OnInstanceStateChanged;
    }

    private void OnInstanceStateChanged(Guid instanceId) => 
        InstanceStateChanged?.Invoke(
            this, 
            new InstanceStateChangedEventArgs(instanceId)
            );

    private async Task AddAsync(InstanceSnapshot snapshot)
    {
        await _configService.ChangeAsync(context => context.AddInstance(snapshot));
        _instanceManager.Register(snapshot.Id);
        
        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, newSnapshot: snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    private async Task UpdateAsync(InstanceSnapshot snapshot)
    {
        var oldSnapshot = GetConfigSnapshot(snapshot.Id);
        
        await _configService.ChangeAsync(context => context.UpdateInstance(snapshot));
        
        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, oldSnapshot, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
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
            draft.ShowCommandHotKey
        );

        if (draft.Id is null)
            await AddAsync(snapshot);
        else
            await UpdateAsync(snapshot);
    }
    
    public async Task RemoveAsync(Guid id)
    {
        var snapshot = GetConfigSnapshot(id);
        
        await _configService.ChangeAsync(context => context.RemoveInstance(id));
        _instanceManager.Remove(id);
        
        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    public InstanceSnapshot GetConfigSnapshot(Guid id)
    {
        return _configService.Config.GetInstance(id);
    }

    public IReadOnlyList<InstanceSnapshot> GetConfigSnapshots()
    {
        return _configService.Config.GetAllInstances();
    }

    public RuntimeSnapshot GetRuntimeSnapshot(Guid id)
    {
        return _instanceManager.GetRuntimeSnapshot(id);
    }

    private InstanceSummary CreateSummary(InstanceSnapshot snapshot, RuntimeSnapshot runtimeSnapshot)
    {
        var status = InstanceStatusMapper.Map(runtimeSnapshot);
        var issues = _validator.Validate(snapshot);
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
        var instances = _instanceManager.GetAllRuntimeStates().ToDictionary(i => i.Id);
        return _configService.Config.GetAllInstances().Select(i =>
            {
                var runtimeSnapshot = instances[i.Id];
                return CreateSummary(i, runtimeSnapshot);
            }).ToList();
    }

    public Task<int> GetActiveCountAsync() => _instanceManager.GetActiveCountAsync();

    private string? ResolveDisplayId(DisplaySelection display)
    {
        var isFallbackAllowed = _configService.Config.GetGlobalSettings().FallbackToPrimaryDisplayIfInvalid;
        return DisplayResolver.ResolveDisplayId(display, isFallbackAllowed);
    }

    public async Task LaunchAsync(Guid id)
    {
        var settings = _configService.Config.GetGlobalSettings();
        
        var snapshot = GetConfigSnapshot(id);
        AuthenticationContext authenticationContext = new OfflineAuthenticationContext();
        if (snapshot.IsOnlineMode)
        {
            var account = _configService.Config.GetAccount(snapshot.AccountId!.Value);
            var region = _configService.Config.GetRegion(snapshot.RegionId!.Value);
            
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
        
        await _instanceManager.LaunchAsync(snapshot.Id, engineLaunchContext);
    }

    public async Task StopAsync(Guid id) => await _instanceManager.StopAsync(id);

    public Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync() => 
        _instanceManager.RequestGracefulShutdownAllAsync();

    public void Show(Guid id) => _instanceManager.Show(id);
}
