using System;
using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record RuntimeSnapshot(Guid Id, State State, ProcessManager? Process, ProcessExitResult? ProcessExitResult, ImmutableArray<ErrorEvent> Errors)
{
    public bool IsActive => State != State.Inactive;
}