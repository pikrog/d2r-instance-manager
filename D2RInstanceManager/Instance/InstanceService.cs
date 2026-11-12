using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using D2RInstanceManager.Config;
using D2RInstanceManager.Display;
using D2RInstanceManager.Engine;
using D2RInstanceManager.Engine.Models;
using D2RInstanceManager.Engine.Models.StateMachine;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;
using D2RInstanceManager.Log;
using D2RInstanceManager.Log.Instance;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Instance;

public class InstanceService : IInstanceConfigProvider
{
    private readonly ConfigService _configService;

    private readonly InstanceConfigValidator _validator;

    private readonly InstanceManager _instanceManager;

    private readonly InstanceNameRegistry _instanceNameRegistry;
    
    private readonly InstanceLaunchContextFactory _launchContextFactory;
    
    private readonly InstanceActivityTracker _activityTracker;

    private readonly InstanceExitReporter _exitReporter;

    private readonly ILogger<InstanceService> _logger;

    public event EventHandler<RuntimeSnapshot>? InstanceStateChanged;
    
    public event EventHandler<InstanceConfigChangedEventArgs>? InstanceConfigChanged;

    public event EventHandler<int>? ActiveCountChanged;

    public int CachedActiveCount => _activityTracker.ActiveCount;
    
    public InstanceService(ConfigService configService,
        InstanceConfigValidator validator,
        InstanceManager instanceManager,
        InstanceNameRegistry instanceNameRegistry,
        InstanceLaunchContextFactory launchContextFactory,
        InstanceActivityTracker activityTracker,
        InstanceExitReporter exitReporter,
        ILogger<InstanceService> logger)
    {
        _configService = configService;
        _validator = validator;
        _instanceManager = instanceManager;
        _instanceNameRegistry = instanceNameRegistry;
        _launchContextFactory = launchContextFactory;
        _activityTracker = activityTracker;
        _exitReporter = exitReporter;
        _logger = logger;

        _instanceManager.InstanceStateChanged += OnInstanceStateChanged;
    }

    private void OnInstanceStateChanged(object? sender, RuntimeSnapshot snapshot)
    {
        var transition = _activityTracker.Update(snapshot.Id, snapshot.IsActive);

        if (transition.BecameInactive)
            _exitReporter.Report(snapshot);

        InstanceStateChanged?.Invoke(this, snapshot);

        if (transition.NewActiveCount is { } activeCount)
            ActiveCountChanged?.Invoke(this, activeCount);
    }

    private async Task AddAsync(InstanceSnapshot snapshot, DisplaySnapshot? displaySnapshotToCache)
    {
        await _configService.ChangeAsync(context =>
        {
            context.AddInstance(snapshot);
            
            if (displaySnapshotToCache is not null)
                context.SaveDisplay(displaySnapshotToCache);
        });
        _instanceNameRegistry.Set(snapshot.Id, snapshot.Name);
        _instanceManager.Register(snapshot.Id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, newSnapshot: snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);
    }

    private async Task UpdateAsync(InstanceSnapshot snapshot, DisplaySnapshot? displaySnapshotToCache)
    {
        var oldSnapshot = GetConfigSnapshot(snapshot.Id);

        await _configService.ChangeAsync(context =>
        {
            context.UpdateInstance(snapshot);
            
            if (displaySnapshotToCache is not null)
                context.SaveDisplay(displaySnapshotToCache);
        });
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
            await AddAsync(snapshot, draft.DisplaySnapshotToCache);
        else
            await UpdateAsync(snapshot, draft.DisplaySnapshotToCache);
    }
    
    public async Task RemoveAsync(Guid id)
    {
        var snapshot = GetConfigSnapshot(id);

        await _configService.ChangeAsync(context => context.RemoveInstance(id));
        _instanceNameRegistry.Remove(id);
        _instanceManager.Remove(id);
        
        var newActiveCount = _activityTracker.Remove(snapshot.Id);

        var eventArgs = new InstanceConfigChangedEventArgs(snapshot.Id, snapshot);
        InstanceConfigChanged?.Invoke(this, eventArgs);

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
        
        _logger.LogInformation("Instance launch requested");

        var preparation = _launchContextFactory.Create(snapshot);
        if (!preparation.IsSuccess)
            return preparation.Error;

        await _instanceManager.LaunchAsync(snapshot.Id, preparation.Value);
        
        return null;
    }

    public async Task StopAsync(Guid id) => await _instanceManager.StopAsync(id);

    public Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync() => 
        _instanceManager.RequestGracefulShutdownAllAsync();

    public void Show(Guid id)
    {
        var centerMouseCursor = _configService.Config.GetGlobalSettings().CenterMouseCursorInShownWindow;
        
        _instanceManager.Show(id, centerMouseCursor);
    }
}