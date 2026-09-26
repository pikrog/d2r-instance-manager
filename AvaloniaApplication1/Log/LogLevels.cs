using System;

namespace AvaloniaApplication1.Log;

public static class LogLevels
{
    public static Array Values => Enum.GetValues<LogLevel>();
    
    public static bool IsEnabled(LogLevel logLevel, LogLevel minimumLevel) => logLevel >= minimumLevel;
    
    public static bool IsVisible(LogLevel entryLevel, LogLevel minimumLevel) => IsEnabled(entryLevel, minimumLevel);
}