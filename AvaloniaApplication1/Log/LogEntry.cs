using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public sealed record LogEntry(
    DateTimeOffset Timestamp,
    LogLevel Level,
    string? Source,
    string Message,
    Exception? Exception,
    IReadOnlyDictionary<string, LogValue> Properties
)
{
    public string? SourceModule => Source?.Split('.')[^1];

    public string? InstanceName =>
        (Properties.GetValueOrDefault(LogProperties.InstanceName) as LogValue.Scalar)?.Value as string;
}