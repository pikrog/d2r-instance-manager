using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Results;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record RuntimeSnapshot(Guid Id, State State, Process? Process, ImmutableArray<ErrorEvent> Errors)
{
    public bool IsActive => State != State.Inactive;
}