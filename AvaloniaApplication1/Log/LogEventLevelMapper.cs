using System;
using Serilog.Events;

namespace AvaloniaApplication1.Log;

public static class LogEventLevelMapper
{
    public static LogEventLevel Map(LogLevel logLevel) => logLevel switch
    {
        LogLevel.Verbose => LogEventLevel.Verbose,
        LogLevel.Debug => LogEventLevel.Debug,
        LogLevel.Information => LogEventLevel.Information,
        LogLevel.Warning => LogEventLevel.Warning,
        LogLevel.Error => LogEventLevel.Error,
        LogLevel.Fatal => LogEventLevel.Fatal,
        _ => throw new InvalidOperationException($"Invalid log level: {logLevel}")
    };
}