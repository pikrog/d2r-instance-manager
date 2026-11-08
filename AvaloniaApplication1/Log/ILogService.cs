using System.Collections.Generic;
using System.Threading;

namespace D2RInstanceManager.Log;

public interface ILogService
{
    bool IsEntryVisible(LogEntry entry, LogFilter filter);
    
    IReadOnlyList<LogEntry> Filter(IEnumerable<LogEntry> logEntries,
        LogFilter filter,
        CancellationToken cancellationToken = default);
}