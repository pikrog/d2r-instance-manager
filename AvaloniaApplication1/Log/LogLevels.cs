using System;

namespace AvaloniaApplication1.Log;

public static class LogLevels
{
    public static Array Values => Enum.GetValues<LogLevel>();
    
    public static bool IsVisible(LogLevel entryLevel, LogLevel minimumLevel) => entryLevel >= minimumLevel;
}