using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Serilog.Events;
using Serilog.Templates;

namespace AvaloniaApplication1.Log;

public static class LogEntryMapper
{
    private static readonly ExpressionTemplate MessageTemplate = new("{@m}");

    private static string RenderMessage(LogEvent logEvent)
    {
        var writer = new StringWriter();
        MessageTemplate.Format(logEvent, writer);
        return writer.ToString();
    }
    
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

    private static LogValue UnwrapPropertyValue(LogEventPropertyValue value) =>
        value switch
        {
            ScalarValue scalar => new LogValue.Scalar(scalar.Value),
            SequenceValue sequence => new LogValue.Sequence(sequence.Elements.Select(UnwrapPropertyValue).ToList()),
            DictionaryValue dictionary =>
                new LogValue.Dictionary(
                dictionary.Elements.ToDictionary(
                    kv => UnwrapPropertyValue(kv.Key)?.ToString() ?? string.Empty, 
                    kv => UnwrapPropertyValue(kv.Value)
                )),
            StructureValue structure =>
                new LogValue.Dictionary(
                    structure.Properties.ToDictionary(
                        p => p.Name, 
                        p => UnwrapPropertyValue(p.Value)
                )),
            _ => new LogValue.Scalar(value.ToString())
        };
    
    private static Dictionary<string, LogValue> MapProperties(
        IReadOnlyDictionary<string, LogEventPropertyValue> properties
        ) => 
        properties.ToDictionary(
            kv => kv.Key, 
            kv => UnwrapPropertyValue(kv.Value)
            );
    
    public static LogEntry Map(LogEvent logEvent)
    {
        var level = MapLevel(logEvent.Level);
        var source = (logEvent.Properties.GetValueOrDefault("SourceContext") as ScalarValue)?.Value?.ToString();
        var message = RenderMessage(logEvent);
        var properties = MapProperties(logEvent.Properties);
        
        return new LogEntry(
            logEvent.Timestamp,
            level,
            source,
            message,
            logEvent.Exception,
            properties
        );
    }
}