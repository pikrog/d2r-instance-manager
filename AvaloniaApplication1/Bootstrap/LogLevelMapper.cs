using System;
using AvaloniaApplication1.CommandLine;
using AvaloniaApplication1.Log;

namespace AvaloniaApplication1.Bootstrap;

public static class LogLevelMapper
{
    public static LogLevel Map(CliLogLevel logLevel) => logLevel switch
    {
        CliLogLevel.Verbose => LogLevel.Verbose,
        CliLogLevel.Debug => LogLevel.Debug,
        CliLogLevel.Information => LogLevel.Information,
        _ => throw new InvalidOperationException($"Invalid CLI log level: {logLevel}")
    };
}