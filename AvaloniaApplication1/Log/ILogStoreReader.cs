using System;
using System.Collections.Generic;

namespace D2RInstanceManager.Log;

public interface ILogStoreReader
{
    IReadOnlyList<LogEntry> Entries { get; }
    
    LogSubscription Subscribe(Action<LogEntry> onNewEntry);
}