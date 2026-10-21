using System;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Toast;

public static class ToastSemanticTypeMapper
{
    public static SemanticType Map(ToastSeverity severity) => severity switch
    {
        ToastSeverity.Info => SemanticType.Info,
        ToastSeverity.Success => SemanticType.Success,
        ToastSeverity.Warning => SemanticType.Warning,
        ToastSeverity.Error => SemanticType.Danger,
        _ => throw new ArgumentOutOfRangeException(nameof(severity), severity, null)
    };
}