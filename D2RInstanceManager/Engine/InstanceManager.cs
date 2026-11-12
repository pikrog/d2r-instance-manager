using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Exceptions;
using D2RInstanceManager.Engine.Factories;
using D2RInstanceManager.Engine.Models;
using D2RInstanceManager.Engine.Models.Contexts.Launch;
using D2RInstanceManager.Engine.Models.StateMachine;
using D2RInstanceManager.Engine.Platform;

namespace D2RInstanceManager.Engine;

public class InstanceManager(InstanceEngineFactory engineFactory)
{
    public event EventHandler<RuntimeSnapshot>? InstanceStateChanged;
    
    private readonly Dictionary<Guid, InstanceEngine> _instances = [];
    
    public void Register(Guid id)
    {
        if (_instances.TryGetValue(id, out _))
            throw new InstanceAlreadyExistsException(id);
        
        var instance = engineFactory.Create(id);
        instance.StateChanged += OnInstanceStateChanged;
        _instances[id] = instance;
    }

    private void OnInstanceStateChanged(object? sender, RuntimeSnapshot snapshot)
    {
        InstanceStateChanged?.Invoke(this, snapshot);
    }

    public void Remove(Guid id)
    {
        if (!_instances.TryGetValue(id, out var instance))
            return;
        
        instance.StateChanged -= OnInstanceStateChanged;
        
        _instances.Remove(id);
    }

    private InstanceEngine Get(Guid id)
    {
        _instances.TryGetValue(id, out var instance);
        return instance ?? throw new InstanceNotFoundException(id);
    }

    public RuntimeSnapshot GetRuntimeSnapshot(Guid id)
    {
        var instance = Get(id);
        return instance.RuntimeSnapshot;
    }
    
    public IReadOnlyList<RuntimeSnapshot> GetAllRuntimeStates() =>
        _instances.Select(p => p.Value.RuntimeSnapshot).ToList().AsReadOnly();
    
    public async Task<int> GetActiveCountAsync()
    {
        await FlushAll();
        return _instances.Values.Count(e => e.RuntimeSnapshot.IsActive);
    }

    public async Task LaunchAsync(Guid id, EngineLaunchContext context)
    {
        var instance = Get(id);
        await instance.LaunchAsync(context);
    }

    public async Task StopAsync(Guid id)
    {
        var instance = Get(id);
        await instance.StopAsync();
    }

    public async Task<IReadOnlyList<ShutdownRequest>> RequestGracefulShutdownAllAsync()
    {
        await FlushAll();
        return _instances.Values.Select(e =>
        {
            var trackProgress = e.RuntimeSnapshot.IsActive;
            return new ShutdownRequest(e.GracefulShutdownAsync(), trackProgress);
        }).ToList();
    }

    private Task FlushAll() => Task.WhenAll(_instances.Values.Select(e => e.FlushAsync()));
    
    public void Show(Guid id, bool centerMouseCursor)
    {
        var snapshot = GetRuntimeSnapshot(id);

        if (snapshot.Process is not { } process)
            return;
        
        var window = process.MainWindowHandle;
        
        Window.BringToTop(window);
        
        if (!centerMouseCursor)
            return;
        
        Window.CenterMouseCursorInside(window);
    }
}
