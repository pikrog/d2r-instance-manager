using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public sealed record CliAuthenticationContext(string AccountUsername, RedactedString AccountPassword, string RegionAddress) : AuthenticationContext;