using System;
using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Models.Results;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record Session
{
    public State State { get; init; } = State.Inactive;
    public LaunchLease? Lease { get; init; }
    public EnginePolicies? Policies { get; init; }
    public ProcessStartInfo? ProcessStartInfo { get; init; }
    public Process? Process { get; init; }
    public uint? ExitCode { get; init; }
    public CleanupState CleanupState { get; init; } = new();
    public ImmutableArray<ErrorEvent> ErrorEvents { get; init; } = [];

    public Session RequireCleanup(CleanupItem item) =>
        this with { CleanupState = CleanupState.Require(item) };

    public Session CompleteCleanup(CleanupItem item) =>
        this with { CleanupState = CleanupState.Complete(item) };

    public Session AddErrorEvent(ErrorEvent @event) => 
        this with { ErrorEvents = ErrorEvents.Add(@event) };
}