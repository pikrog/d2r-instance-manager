using System;
using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication1.Log;

public class LogService : ILogService
{
    private static bool IsEntryVisibleByName(string? name, IReadOnlySet<string> names, bool includeNoName)
    {
        if(!includeNoName && names.Count == 0)
            return true;
        
        return string.IsNullOrEmpty(name) 
            ? includeNoName 
            : names.Contains(name);
    }

    private static bool PropertyValueContainsText(LogValue value, string text) => value switch
    {
        LogValue.Dictionary { Properties: { } d } => d.Values.Any(p => PropertyValueContainsText(p, text)),
        LogValue.Sequence { Items: { } i } => i.Any(p => PropertyValueContainsText(p, text)),
        LogValue.Scalar { Value: { } v } => v.ToString()?.Contains(text, StringComparison.OrdinalIgnoreCase) ?? false,
        _ => false
    };

    private static bool PropertiesContainText(IReadOnlyDictionary<string, LogValue> properties, string text) =>
        properties.Values.Any(p => PropertyValueContainsText(p, text));
    
    private static bool IsEntryVisibleBySearchText(LogEntry entry, string? searchText) =>
        string.IsNullOrEmpty(searchText) 
        || entry.Message.Contains(searchText, StringComparison.OrdinalIgnoreCase) 
        || PropertiesContainText(entry.Properties, searchText);

    public bool IsEntryVisible(LogEntry entry, LogFilter filter) =>
        LogLevels.IsVisible(entry.Level, filter.Level)
        && IsEntryVisibleByName(entry.InstanceName, filter.InstanceNames, filter.IncludeNoInstance)
        && IsEntryVisibleByName(entry.SourceModule, filter.SourceModules, filter.IncludeNoSourceModule)
        && IsEntryVisibleBySearchText(entry, filter.SearchText);
    
    public IReadOnlyList<LogEntry> Filter(IEnumerable<LogEntry> logEntries, LogFilter filter) => 
        logEntries.Where(e => IsEntryVisible(e, filter))
            .ToList();
}