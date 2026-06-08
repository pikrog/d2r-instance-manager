using System;
using System.Collections.Generic;
using AvaloniaApplication1.Engine.Models.Effects;
using AvaloniaApplication1.Engine.Models.SessionOperations;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public class TransitionResult(State nextState, IReadOnlyList<Effect>? effects = null, IReadOnlyList<SessionOperation>? sessionOperations = null)
{
    public State NextState { get; } = nextState;
    public IReadOnlyList<Effect> Effects { get; } = effects ?? [];
    public IReadOnlyList<SessionOperation> SessionOperations { get; } = sessionOperations ?? [];
    
    public void Deconstruct(out State nextState, out IReadOnlyList<Effect> effects, out IReadOnlyList<SessionOperation> sessionOperations)
    {
        nextState = this.NextState;
        effects = this.Effects;
        sessionOperations = this.SessionOperations;
    }
    
    public bool IsNoOp(State prevState) => NextState == prevState && Effects.Count == 0 && SessionOperations.Count == 0;
}