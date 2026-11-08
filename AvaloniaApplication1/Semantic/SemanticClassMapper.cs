using System;

namespace D2RInstanceManager.Semantic;

public static class SemanticClassMapper
{
    public static string Map(SemanticType type) => type switch
    {
        SemanticType.Neutral => "neutral",
        SemanticType.Success => "success",
        SemanticType.Warning => "warning",
        SemanticType.Danger => "danger",
        SemanticType.Info => "info",
        _ => throw new InvalidOperationException($"Unexpected semantic type: {type.GetType().Name}")
    };
}