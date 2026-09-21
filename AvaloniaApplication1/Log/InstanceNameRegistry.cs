using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public class InstanceNameRegistry
{
    private readonly ConcurrentDictionary<Guid, string> _names = new();

    public void Set(Guid id, string name) => _names[id] = name;

    public void Remove(Guid id) => _names.TryRemove(id, out _);

    public string? Get(Guid id) => _names.GetValueOrDefault(id);
}