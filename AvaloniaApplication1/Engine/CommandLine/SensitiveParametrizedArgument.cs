using D2RInstanceManager.Engine.Common;

namespace D2RInstanceManager.Engine.CommandLine;

public sealed record SensitiveParametrizedArgument(string Name, RedactedString Value) : Argument;