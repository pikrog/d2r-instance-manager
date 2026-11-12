using System;
using System.Collections.Generic;
using System.Threading;
using D2RInstanceManager.Instance.Models;

namespace D2RInstanceManager.Instance;

public sealed class InstanceActivityTracker
{
    private readonly Lock _lock = new();

    private readonly Dictionary<Guid, bool> _states = [];

    private int _activeCount;

    public int ActiveCount
    {
        get
        {
            lock (_lock)
                return _activeCount;
        }
    }
    
    public ActivityTransition Update(Guid id, bool isActive)
    {
        lock (_lock)
        {
            var wasActive = _states.GetValueOrDefault(id);
            _states[id] = isActive;

            if (wasActive == isActive)
                return new ActivityTransition(wasActive, isActive, null);

            _activeCount += isActive ? 1 : -1;
            return new ActivityTransition(wasActive, isActive, _activeCount);
        }
    }

    public int? Remove(Guid id)
    {
        lock (_lock)
        {
            return _states.Remove(id, out var wasActive) && wasActive
                ? --_activeCount
                : null;
        }
    }
}