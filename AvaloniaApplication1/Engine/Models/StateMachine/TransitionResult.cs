using System;
using System.Collections.Generic;
using AvaloniaApplication1.Engine.Models.Effects;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public sealed record TransitionResult(Session Session, IReadOnlyList<Effect> Effects)
{
    public TransitionResult(Session session) : this(session, Array.Empty<Effect>())
    {
    }
}