namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public sealed record CliAuthenticationContext(string AccountUsername, string AccountPassword, string RegionAddress) : AuthenticationContext;