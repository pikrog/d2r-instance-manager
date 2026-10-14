using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;
using AvaloniaApplication1.Log;
using AvaloniaApplication1.Log.Instance;
using AvaloniaApplication1.Notification;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Instance;

public class InstanceService : IInstanceConfigProvider
{
    private readonly ConfigService _configService;

    private readonly InstanceConfigValidator _validator;

    private readonly InstanceManager _instanceManager;

    private readonly InstanceNameRegistry _instanceNameRegistry;
    
    private readonly InstanceLaunchContextFactory _launchContextFactory;
    
    private readonly INotificationSink _notificationSink;

    private readonly ILogger<InstanceService> _logger;

    public event EventHandler<RuntimeSnapshot>? InstanceStateChanged;
    
    public event EventHandler<InstanceConfigChangedEventArgs>? InstanceConfigChanged;

    private readonly Lock _instanceActivityStatesLock = new();
    
    private readonly Dictionary<Guid, bool> _instanceActivityStates = [];

    private int _cachedActiveCount;

    public int CachedActiveCount
    {
        get
        {
            lock (_instanceActivityStatesLock)
                return _cachedActiveCount;
        }
    }
    
    public event EventHandler<int>? ActiveCountChanged;

    public InstanceService(ConfigService configService,
        InstanceConfigValidator validator,
        InstanceManager instanceManager,
        InstanceNameRegistry instanceNameRegistry,
        InstanceLaunchContextFactory launchContextFactory,
        [FromKeyedServices(NotificationCounterKey.Instances)] INotificationSink notificationSink,
        ILogger<InstanceService> logger)
    {
        _configService = configService;
        _validator = validator;
        _instanceManager = instanceManager;
        _instanceNameRegistry = instanceNameRegistry;
        _launchContextFactory = launchContextFactory;
        _notificationSink = notificationSink;
        _logger = logger;

        _instanceManager.InstanceStateChanged += OnInstanceStateChanged;
    }

    private static bool HasInstanceFailed(RuntimeSnapshot snapshot) =>
        snapshot.Errors.Any() || snapshot.ProcessExitResult is not ProcessExitResult.Success;

    private void OnInstanceStateChanged(object? sender, RuntimeSnapshot snapshot)
    {
        bool previousActivityState;
        int? newActiveCount = null;

        lock (_instanceActivityStatesLock)
        {
            previousActivityState = _instanceActivityStates.GetValueOrDefault(snapshot.Id);
            _instanceActivityStates[snapshot.Id] = snapshot.IsActive;

            if (previousActivityState != snapshot.IsActive)
            {
                _cachedActiveCount += snapshot.IsActive ? 1 : -1;
                newActiveCount = _cachedActiveCount;
            }
        }

        if (previousActivityState && !snapshot.IsActive && HasInstanceFailed(snapshot))
            _notificationSink.Increment();

        if (previousActivityState && !snapshot.IsActive)
        {
            var instanceName = _instanceNameRegistry.Get(snapshot.Id);
            using var _ = _logger.BeginScope(new Dictionary<string, object?>{{ LogProperties.InstanceName, instanceName }});
            
            if (HasInstanceFailed(snapshot))
                _logger.LogError("Instance failed");
            else
                _logger.LogInformation("Instance stopped");
        }

        InstanceStateChanged?.Invoke(this, snapshot);
        
        if (newActiveCount is { } activeCount)
            ActiveCountChanged?.Invoke(this, activeCount);
    }

    private async Task AddAsync(InstanceSnapshot snapshot)
    {
        await _configService.ChangeAsync(context => context.AddInstance(snapshot));
        _instanceNameRegistry.Set(snapshot.Id, snapshot.Name);
        _instanceManager.Register(snapshot.Id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, newSnapshot: snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    private async Task UpdateAsync(InstanceSnapshot snapshot)
    {
        var oldSnapshot = GetConfigSnapshot(snapshot.Id);

        await _configService.ChangeAsync(context => context.UpdateInstance(snapshot));
        _instanceNameRegistry.Set(snapshot.Id, snapshot.Name);

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
        _instanceNameRegistry.Remove(id);
        _instanceManager.Remove(id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);

        int? newActiveCount = null;
        lock (_instanceActivityStatesLock)
        {
            if (_instanceActivityStates.Remove(snapshot.Id, out var wasActive) && wasActive)
                newActiveCount = --_cachedActiveCount;
        }
        if (newActiveCount is { } count)
            ActiveCountChanged?.Invoke(this, count);
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

    public async Task<InstanceLaunchFailure?> LaunchAsync(Guid id)
    {
        var snapshot = GetConfigSnapshot(id);

        using var _ = _logger.BeginScope(new Dictionary<string, object> { { LogProperties.InstanceName, snapshot.Name } });

        var preparation = _launchContextFactory.Create(snapshot);
        if (!preparation.IsSuccess)
            return preparation.Error;

        await _instanceManager.LaunchAsync(snapshot.Id, preparation.Value);
        
        _logger.LogInformation("Instance launch requested");

        return null;
    }

    public async Task StopAsync(Guid id) => await _instanceManager.StopAsync(id);

    public Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync() => 
        _instanceManager.RequestGracefulShutdownAllAsync();

    public void Show(Guid id) => _instanceManager.Show(id);
}
