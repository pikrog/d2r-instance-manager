using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Log;

public class LogLevelToSemanticTypeConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not LogLevel level)
            return AvaloniaProperty.UnsetValue;
        
        return level switch
        {
            LogLevel.Verbose or LogLevel.Debug => SemanticType.Neutral,
            LogLevel.Error or LogLevel.Fatal => SemanticType.Danger,
            LogLevel.Information => SemanticType.Info,
            LogLevel.Warning => SemanticType.Warning,
            _ => AvaloniaProperty.UnsetValue
        };
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}