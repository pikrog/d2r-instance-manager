using D2RInstanceManager.Engine.Models.Contexts.Launch;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record Authenticate(AuthenticationContext AuthenticationContext) : Effect;