using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record Authenticate(AuthenticationContext AuthenticationContext) : Effect;