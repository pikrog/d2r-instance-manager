using System.Collections.Generic;

namespace D2RInstanceManager.Log;

public abstract record LogValue
{
    public sealed record Scalar(object? Value) : LogValue;

    public sealed record Dictionary(IReadOnlyDictionary<string, LogValue> Properties) : LogValue;

    public sealed record Sequence(IReadOnlyList<LogValue> Items) : LogValue;
};