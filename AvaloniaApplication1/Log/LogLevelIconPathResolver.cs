using System;

namespace AvaloniaApplication1.Log;

public static class LogLevelIconPathResolver
{
    public static string Resolve(LogLevel level) => level switch
    {
        LogLevel.Fatal => "/Assets/images/fatal.svg",
        LogLevel.Error => "/Assets/images/error.svg",
        LogLevel.Warning => "/Assets/images/warning.svg",
        LogLevel.Information => "/Assets/images/info.svg",
        LogLevel.Debug => "/Assets/images/debug.svg",
        LogLevel.Verbose => "/Assets/images/verbose.svg",
        _ => throw new InvalidOperationException($"Unexpected log level: {level}")
    };
}