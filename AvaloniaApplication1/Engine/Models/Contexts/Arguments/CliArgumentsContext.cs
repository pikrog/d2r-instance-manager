using D2RInstanceManager.Engine.Common;

namespace D2RInstanceManager.Engine.Models.Contexts.Arguments;

public sealed record CliArgumentsContext(string AccountUsername, RedactedString AccountPassword, string RegionAddress) : AuthenticationArgumentsContext;