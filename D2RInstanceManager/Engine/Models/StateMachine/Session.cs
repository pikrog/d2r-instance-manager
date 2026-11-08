using System;
using System.Collections.Immutable;
using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Coordination;
using D2RInstanceManager.Engine.Models.Contexts;
using D2RInstanceManager.Engine.Models.Contexts.Launch;
using D2RInstanceManager.Engine.Models.Events;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.StateMachine;

public sealed record Session : IDisposable
{
    public State State { get; init; } = State.Inactive;
    public bool IsShuttingDown { get; init; }
    public LaunchLease? Lease { get; init; }
    public EnginePolicies? Policies { get; init; }
    public ProcessStartContext? ProcessStartContext { get; init; }
    public ProcessManager? Process { get; init; }
    public ProcessExitResult? ProcessExitResult { get; init; }
    public CleanupState CleanupState { get; init; } = new();
    public ImmutableArray<ErrorEvent> ErrorEvents { get; init; } = [];

    public Session RequireCleanup(CleanupItem item) =>
        this with { CleanupState = CleanupState.Require(item) };

    public Session CompleteCleanup(CleanupItem item) =>
        this with { CleanupState = CleanupState.Complete(item) };

    public Session AddErrorEvent(ErrorEvent @event) => 
        this with { ErrorEvents = ErrorEvents.Add(@event) };

    public void Dispose()
    {
        Lease?.Dispose();
        Process?.Dispose();
    }
}