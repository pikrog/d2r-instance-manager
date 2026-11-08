using System;
using D2RInstanceManager.CommandLine;
using D2RInstanceManager.Log;

namespace D2RInstanceManager.Bootstrap;

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