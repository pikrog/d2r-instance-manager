using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.Models.Contexts.Arguments;

public sealed record CliArgumentsContext(string AccountUsername, RedactedString AccountPassword, string RegionAddress) : AuthenticationArgumentsContext;