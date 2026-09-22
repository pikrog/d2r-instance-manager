using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Effects;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.StateMachine;

namespace AvaloniaApplication1.Engine;

public static class InstanceStateMachine
{
    public static TransitionResult Apply(Session session, Event @event)
    {
        var result = ApplyTransition(session, @event);
        result = AddErrorEvent(result, @event);
        return CompleteCleanupIfNeeded(result);
    }

    private static TransitionResult ApplyTransition(Session session, Event @event)
    {
        switch (session.State, @event) // todo: handle UnexpectedError (-> stop instance)
        {
            case (State.Inactive, LaunchRequested e):
                return StartNewSession(e);
            case (State.Inactive, ShutdownRequested):
                return To(session, State.Shutdown);

            case (State.Authenticating, Authenticated):
                return To(
                    session.RequireCleanup(CleanupItem.LaunchLease),
                    State.WaitingForStart,
                    [new AcquireLaunchLease()]
                    );
            case (State.Authenticating, AuthenticationFailed):
                return To(session, State.Stopping);
            case (State.Authenticating, StopRequested):
                return To(session, State.Stopping, [new Cancel()]);
            case (State.Authenticating, ShutdownRequested):
                return To(session with { IsShuttingDown = true }, State.Stopping, [new Cancel()]);

            case (State.WaitingForStart, LaunchLeaseGranted e):
                return To(
                    (session with { Lease = e.Lease }).RequireCleanup(CleanupItem.Process),
                    State.Starting,
                    [new StartProcess(Require(session.ProcessStartContext))]
                    );
            case (State.WaitingForStart, LaunchLeaseCanceled):
                return To(session.CompleteCleanup(CleanupItem.LaunchLease), State.Stopping);
            case (State.WaitingForStart, StopRequested):
                return To(session, State.Stopping, [new Cancel()]);
            case (State.WaitingForStart, ShutdownRequested):
                return To(session with { IsShuttingDown = true }, State.Stopping, [new Cancel()]);

            case (State.Starting, ProcessStarted e):
                return To(
                    session with { Process = e.ProcessManager },
                    State.WaitingForUnlock,
                    [
                        new MonitorProcessExit(e.ProcessManager, Require(session.Policies).ProcessStopPolicies.ForcefulExitCode),
                        new UnlockMultibox(
                            Require(session.Policies?.UnlockMultiboxRetryPolicy), 
                            e.ProcessManager.Id)
                    ]);
            case (State.Starting, ProcessStartFailed):
                return To(
                    session.CompleteCleanup(CleanupItem.Process),
                    State.Stopping,
                    [new ReleaseLaunchLease(Require(session.Lease))]
                    );
            case (State.Starting, StopRequested):
                return To(
                    session,
                    State.Stopping,
                    [new Cancel()]);
            case (State.Starting, ShutdownRequested):
                return To(session with { IsShuttingDown = true }, State.Stopping, [new Cancel()]);

            case (State.WaitingForUnlock, MultiboxUnlocked):
                return To(
                    session,
                    State.Running,
                    [new ReleaseLaunchLease(Require(session.Lease))]
                    );
            case (State.WaitingForUnlock, MultiboxUnlockFailed):
            case (State.WaitingForUnlock, StopRequested):
                return To(
                    session,
                    State.Stopping,
                    [
                        new StopProcess(Require(session.Process), Require(session.Policies?.ProcessStopPolicies)),
                        new Cancel()
                    ]);
            case (State.WaitingForUnlock, ShutdownRequested):
                return To(
                    session with { IsShuttingDown = true },
                    State.Stopping,
                    [
                        new StopProcess(Require(session.Process), Require(session.Policies?.ProcessStopPolicies)),
                        new Cancel()
                    ]);
            case (State.WaitingForUnlock, ProcessExited e):
                return To(
                    (session with { ProcessExitResult = e.Result }).CompleteCleanup(CleanupItem.Process),
                    State.Stopping,
                    [new Cancel(), new ReleaseLaunchLease(Require(session.Lease))]
                    );

            case (State.Running, LaunchLeaseReleased):
                return To(session.CompleteCleanup(CleanupItem.LaunchLease), State.Running);
            case (State.Running, StopRequested):
                return To(
                    session,
                    State.Stopping,
                    [
                        new StopProcess(Require(session.Process), Require(session.Policies?.ProcessStopPolicies)),
                        new Cancel()
                    ]);
            case (State.Running, ShutdownRequested):
                return To(
                    session with { IsShuttingDown = true },
                    State.Stopping,
                    [
                        new StopProcess(Require(session.Process), Require(session.Policies?.ProcessStopPolicies)),
                        new Cancel()
                    ]);
            case (State.Running, ProcessExited e):
                return To(
                    (session with { ProcessExitResult = e.Result }).CompleteCleanup(CleanupItem.Process),
                    State.Stopping
                    );

            case (State.Stopping, ShutdownRequested):
                return To(session with { IsShuttingDown = true }, State.Stopping);
            case (State.Stopping, ProcessStarted e):
                return To(
                    session with { Process = e.ProcessManager },
                    State.Stopping,
                    [
                        new MonitorProcessExit(e.ProcessManager, Require(session.Policies).ProcessStopPolicies.ForcefulExitCode),
                        new StopProcess(e.ProcessManager, Require(session.Policies?.ProcessStopPolicies))
                    ]);
            case (State.Stopping, LaunchLeaseGranted e):
                return To(session, State.Stopping, [new ReleaseLaunchLease(e.Lease)]);
            case (State.Stopping, ProcessExited e):
                return To(
                    (session with { ProcessExitResult = e.Result }).CompleteCleanup(CleanupItem.Process),
                    State.Stopping,
                    [new ReleaseLaunchLease(Require(session.Lease))]
                    );
            case (State.Stopping, ProcessStartFailed):
            case (State.Stopping, ProcessStopFailed):
                return To(session.CompleteCleanup(CleanupItem.Process), State.Stopping, [new ReleaseLaunchLease(Require(session.Lease))]);
            case (State.Stopping, LaunchLeaseReleased):
            case (State.Stopping, LaunchLeaseCanceled):
                return To(session.CompleteCleanup(CleanupItem.LaunchLease), State.Stopping);

            default:
                return new TransitionResult(session);
        }
    }

    private static TransitionResult StartNewSession(LaunchRequested @event)
    {
        var session = new Session
        {
            ProcessStartContext = @event.ProcessStartContext,
            Policies = @event.EnginePolicies
        };

        return @event.AuthenticationContext switch
        {
            OsiAuthenticationContext => To(
                session,
                State.Authenticating,
                [new Authenticate(@event.AuthenticationContext)]),
            OfflineAuthenticationContext or CliAuthenticationContext => To(
                session.RequireCleanup(CleanupItem.LaunchLease),
                State.WaitingForStart,
                [new AcquireLaunchLease()]),
            _ => throw new InvalidOperationException($"Unexpected authentication context: {@event.AuthenticationContext.GetType().Name}")
        };
    }

    private static TransitionResult AddErrorEvent(TransitionResult result, Event @event) =>
        @event is ErrorEvent errorEvent
            ? result with { Session = result.Session.AddErrorEvent(errorEvent) }
            : result;

    private static TransitionResult CompleteCleanupIfNeeded(TransitionResult result) =>
        result.Session is { State: State.Stopping, CleanupState.IsCleanupComplete: true }
            ? new TransitionResult(
                result.Session with { State = result.Session.IsShuttingDown ? State.Shutdown : State.Inactive },
                [..result.Effects, new Reset()])
            : result;

    private static TransitionResult To(Session session, State state) =>
        new(session with { State = state });

    private static TransitionResult To(Session session, State state, IReadOnlyList<Effect> effects) =>
        new(session with { State = state }, effects);

    private static T Require<T>(T? value, [CallerArgumentExpression(nameof(value))] string name = "") where T : class =>
        value ?? throw new InvalidOperationException($"Session is missing required value: {name}");
}
