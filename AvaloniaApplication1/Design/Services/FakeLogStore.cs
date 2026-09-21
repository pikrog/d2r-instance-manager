using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using AvaloniaApplication1.Log;

namespace AvaloniaApplication1.Design.Services;

public class FakeLogStore : ILogStoreReader
{
    private class DummySubscriptionHandle : IDisposable
    {
        public void Dispose() {}
    }
    
    private static readonly DateTimeOffset StartDateTimeOffset = DateTimeOffset.Now;
    
    private static readonly Dictionary<string, object?> InstanceEnrichedProperties = new(){{LogProperties.InstanceName, "Nova"}};
    
    private readonly List<LogEntry> _entries =
    [
        new(StartDateTimeOffset, LogLevel.Warning, "InstanceService", "Display for instance 'test' is not connected. Falling back to primary", null, InstanceEnrichedProperties),
        new(StartDateTimeOffset.AddSeconds(1), LogLevel.Information, "InstanceService", "Instance started", null, InstanceEnrichedProperties),
        new(StartDateTimeOffset.AddSeconds(2), LogLevel.Debug, "InstanceEngine", "Process PID 1234 started", null, InstanceEnrichedProperties),
        new(StartDateTimeOffset.AddSeconds(3), LogLevel.Error, "InstanceEngine", "Multibox unlocking for instance 'test' timed out", null, InstanceEnrichedProperties),
        new(StartDateTimeOffset.AddSeconds(4), LogLevel.Verbose, "HotKeyBindingCoordinator", "Ctrl+F4 was pressed", null, ReadOnlyDictionary<string, object?>.Empty),
        new(StartDateTimeOffset.AddSeconds(5), LogLevel.Fatal, "", "Unhandled exception", new InvalidOperationException("Channel was completed"), ReadOnlyDictionary<string, object?>.Empty),
    ];
    
    public IReadOnlyList<LogEntry> Entries => _entries;
    
    public LogSubscription Subscribe(Action<LogEntry> onNewEntry) => new(Entries, new DummySubscriptionHandle());
}