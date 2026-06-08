using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record CheckCleanupCompletion(CleanupState CleanupState) : Effect;