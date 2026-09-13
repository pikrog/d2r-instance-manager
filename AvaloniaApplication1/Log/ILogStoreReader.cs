using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public interface ILogStoreReader
{
    IReadOnlyList<LogEntry> Entries { get; }
    
    LogSubscription Subscribe(Action<LogEntry> onNewEntry);
}