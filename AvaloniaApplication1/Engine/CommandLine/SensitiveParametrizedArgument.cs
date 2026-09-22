using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.CommandLine;

public sealed record SensitiveParametrizedArgument(string Name, RedactedString Value) : Argument;