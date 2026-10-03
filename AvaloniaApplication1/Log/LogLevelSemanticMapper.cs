using System;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Log;

public static class LogLevelSemanticMapper
{
    public static SemanticType Map(LogLevel level) => level switch
    {
        LogLevel.Verbose or LogLevel.Debug or LogLevel.Information 
            => SemanticType.Info,
        
        LogLevel.Warning 
            => SemanticType.Warning,
        
        LogLevel.Error or LogLevel.Fatal 
            => SemanticType.Danger,
        
        _ => throw new InvalidOperationException($"Unexpected log level: {level}")
    };
}