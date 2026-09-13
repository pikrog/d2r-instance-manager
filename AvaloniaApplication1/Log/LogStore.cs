using System;
using System.Collections.Generic;
using System.Threading;

namespace AvaloniaApplication1.Log;

public sealed class LogStore : ILogStoreReader, ILogStoreWriter
{
    private sealed class SubscriptionHandle(LogStore store, Action<LogEntry> onNewEntry) : IDisposable
    {
        private bool _disposed;
        
        public void Dispose()
        {
            if (_disposed)
                return;

            _disposed = true;
            
            store.Unsubscribe(onNewEntry);
        }
    }
    
    private readonly Lock _gate = new();

    private readonly List<LogEntry> _entries = [];
    
    public IReadOnlyList<LogEntry> Entries => _entries;
    
    private readonly List<Action<LogEntry>> _subscribers = [];
    
    public LogSubscription Subscribe(Action<LogEntry> onNewEntry)
    {
        lock (_gate)
        {
            var snapshot = _entries.ToArray();
            _subscribers.Add(onNewEntry);
            var handle = new SubscriptionHandle(this, onNewEntry);
            return new LogSubscription(snapshot, handle);
        }
    }

    private void Unsubscribe(Action<LogEntry> onNewEntry)
    {
        lock (_gate)
        {
            _subscribers.Remove(onNewEntry);
        }
    }

    public void Append(LogEntry logEntry)
    {
        Action<LogEntry>[] subscribers;
        
        lock (_gate)
        {
            subscribers = _subscribers.ToArray();
            
            _entries.Add(logEntry);
        }
        
        foreach (var subscriber in subscribers)
            subscriber(logEntry);
    }
}