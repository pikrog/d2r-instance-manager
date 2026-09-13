using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Log;

public sealed record LogEntry(
    DateTimeOffset Timestamp, 
    LogLevel Level, 
    string Message, 
    Exception? Exception, 
    IReadOnlyDictionary<string, object?> Properties
    );