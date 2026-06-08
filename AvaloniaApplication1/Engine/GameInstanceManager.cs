using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Factories;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine;

public class GameInstanceManager(GameInstanceEngineFactory engineFactory)
{
    public event Action<RuntimeSnapshot>? InstanceStateChanged;
    
    private readonly Dictionary<Guid, GameInstanceEngine> _instances = [];
    
    public void Register(Guid id)
    {
        if (_instances.TryGetValue(id, out _))
            throw new GameInstanceAlreadyExistsException(id);
        
        var instance = engineFactory.Create(id);
        instance.StateChanged += OnInstanceStateChanged;
        _instances[id] = instance;
    }

    private void OnInstanceStateChanged(object? sender, RuntimeSnapshot e)
    {
        InstanceStateChanged?.Invoke(e);
    }

    public void Remove(Guid id)
    {
        _instances.Remove(id);
    }

    private GameInstanceEngine Get(Guid id)
    {
        _instances.TryGetValue(id, out var instance);
        return instance ?? throw new GameInstanceNotFoundException($"Instance with id {id} not found");
    }

    public RuntimeSnapshot GetRuntimeState(Guid id)
    {
        var instance = Get(id);
        return instance.RuntimeSnapshot;
    }
    
    public IReadOnlyList<RuntimeSnapshot> GetAllRuntimeStates() =>
        _instances.Select(p => p.Value.RuntimeSnapshot).ToList().AsReadOnly();

    public async Task LaunchAsync(Guid id, LaunchContext context)
    {
        var instance = Get(id);
        await instance.StartAsync(context);
    }
}