using System;
using System.Collections.Immutable;
using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Models.Events;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.StateMachine;

public record RuntimeSnapshot(Guid Id, State State, ProcessManager? Process, ProcessExitResult? ProcessExitResult, ImmutableArray<ErrorEvent> Errors)
{
    public bool IsActive => State != State.Inactive && State != State.Shutdown;
}