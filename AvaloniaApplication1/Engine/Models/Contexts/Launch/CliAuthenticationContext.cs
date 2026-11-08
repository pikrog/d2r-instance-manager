using D2RInstanceManager.Engine.Common;

namespace D2RInstanceManager.Engine.Models.Contexts.Launch;

public sealed record CliAuthenticationContext(string AccountUsername, RedactedString AccountPassword, string RegionAddress) : AuthenticationContext;