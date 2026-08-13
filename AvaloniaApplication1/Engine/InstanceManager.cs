using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine;

public class InstanceManager(InstanceEngineFactory engineFactory)
{
    public event Action<Guid>? InstanceStateChanged;
    
    private readonly Dictionary<Guid, InstanceEngine> _instances = [];
    
    public void Register(Guid id)
    {
        if (_instances.TryGetValue(id, out _))
            throw new InstanceAlreadyExistsException(id);
        
        var instance = engineFactory.Create(id);
        instance.StateChanged += OnInstanceStateChanged;
        _instances[id] = instance;
    }

    private void OnInstanceStateChanged(object? sender, Guid id)
    {
        InstanceStateChanged?.Invoke(id);
    }

    public void Remove(Guid id)
    {
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

    public IReadOnlyList<ShutdownTask> RequestGracefulShutdownAll() =>
        _instances.Values.Select(e =>
        {
            var trackProgress = IsActive(e);
            return new ShutdownTask(e.GracefulShutdownAsync(), trackProgress);
        }).ToList();

    public void Show(Guid id)
    {
        var snapshot = GetRuntimeSnapshot(id);
        snapshot.Process?.BringMainWindowToTop();
    }
}