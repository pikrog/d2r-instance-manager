using Serilog.Core;
using Serilog.Events;

namespace AvaloniaApplication1.Log;

public sealed class InMemoryLogSink(ILogStoreWriter store) : ILogEventSink
{
    public void Emit(LogEvent logEvent)
    {
        var entry = LogEntryMapper.Map(logEvent);
        store.Append(entry);
    }
}