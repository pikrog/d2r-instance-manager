using System;
using System.Collections.Generic;
using System.Linq;
using Serilog.Events;

namespace AvaloniaApplication1.Log;

public static class LogEntryMapper
{
    private static LogLevel MapLevel(LogEventLevel logEventLevel) =>
        logEventLevel switch
        {
            LogEventLevel.Verbose => LogLevel.Verbose,
            LogEventLevel.Debug => LogLevel.Debug,
            LogEventLevel.Information => LogLevel.Information,
            LogEventLevel.Warning => LogLevel.Warning,
            LogEventLevel.Error => LogLevel.Error,
            LogEventLevel.Fatal => LogLevel.Fatal,
            _ => throw new InvalidOperationException($"Unexpected log level: {logEventLevel}")
        };

    private static object? UnwrapPropertyValue(LogEventPropertyValue value) =>
        value switch
        {
            ScalarValue scalar => scalar.Value,
            SequenceValue sequence => sequence.Elements.Select(UnwrapPropertyValue).ToList(),
            DictionaryValue dictionary =>
                dictionary.Elements.ToDictionary(
                    kv => UnwrapPropertyValue(kv.Key)?.ToString() ?? string.Empty,
                    kv => UnwrapPropertyValue(kv.Value)
                ),
            StructureValue structure =>
                structure.Properties.ToDictionary(
                    p => p.Name,
                    p => UnwrapPropertyValue(p.Value)
                ),
            _ => value.ToString()
        };
    
    private static IReadOnlyDictionary<string, object?> MapProperties(
        IReadOnlyDictionary<string, LogEventPropertyValue> properties
        ) => 
        properties.ToDictionary(
            kv => kv.Key, 
            kv => UnwrapPropertyValue(kv.Value)
            );
    
    public static LogEntry Map(LogEvent logEvent) =>
        new(
            logEvent.Timestamp, 
            MapLevel(logEvent.Level), 
            logEvent.MessageTemplate.Render(logEvent.Properties), 
            logEvent.Exception,
            MapProperties(logEvent.Properties)
            );
}