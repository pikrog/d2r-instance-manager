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
    
    private readonly List<LogEntry> _entries =
    [
        new(StartDateTimeOffset,  LogLevel.Warning,"Display for instance 'test' is not connected. Falling back to primary", null, ReadOnlyDictionary<string, object?>.Empty),
        new(StartDateTimeOffset.AddSeconds(1),  LogLevel.Information,"Instance 'test' started", null, ReadOnlyDictionary<string, object?>.Empty),
        new(StartDateTimeOffset.AddSeconds(2),  LogLevel.Debug,"Process PID 1234 started", null, ReadOnlyDictionary<string, object?>.Empty),
        new(StartDateTimeOffset.AddSeconds(3),  LogLevel.Error,"Multibox unlocking for instance 'test' timed out", null, ReadOnlyDictionary<string, object?>.Empty),
    ];
    
    public IReadOnlyList<LogEntry> Entries => _entries;
    
    public LogSubscription Subscribe(Action<LogEntry> onNewEntry) => new(Entries, new DummySubscriptionHandle());
}