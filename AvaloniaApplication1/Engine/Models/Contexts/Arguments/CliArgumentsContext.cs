namespace AvaloniaApplication1.Engine.Models.Contexts.Arguments;

public sealed record CliArgumentsContext(string AccountUsername, string AccountPassword, string RegionAddress) : AuthenticationArgumentsContext;