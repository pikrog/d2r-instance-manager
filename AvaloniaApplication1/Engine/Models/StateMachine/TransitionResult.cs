using System;
using System.Collections.Generic;
using D2RInstanceManager.Engine.Models.Effects;

namespace D2RInstanceManager.Engine.Models.StateMachine;

public sealed record TransitionResult(Session Session, IReadOnlyList<Effect> Effects)
{
    public TransitionResult(Session session) : this(session, Array.Empty<Effect>())
    {
    }
}