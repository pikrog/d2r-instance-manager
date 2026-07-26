using System;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.Instance.Models;

namespace AvaloniaApplication1.Instance;

public static class GameInstanceStatusMapper
{
    public static GameInstanceStatus Map(RuntimeSnapshot snapshot) =>
        snapshot.State switch
        {
            State.Inactive => ResolveInactiveStatus(snapshot),
            State.Authenticating => GameInstanceStatus.Authenticating,
            State.WaitingForStart => GameInstanceStatus.Queued,
            State.Starting or State.WaitingForUnlock => GameInstanceStatus.Starting,
            State.Running => GameInstanceStatus.Running,
            State.Stopping => GameInstanceStatus.Stopping,
            _ => throw new InvalidOperationException($"Unknown state {snapshot.State}")
        };

    private static GameInstanceStatus ResolveInactiveStatus(RuntimeSnapshot snapshot)
    {
        if (snapshot.Errors.Length > 0)
        {
            if (snapshot.Errors[0] is MultiboxUnlockFailed { IsKnown: true, Error: RetryingMultiboxUnlockError.Timeout } 
                || snapshot.Errors[0] is ProcessStopFailed { IsKnown: true, Error: ProcessStopTimeout })
                return GameInstanceStatus.Timeout;
            return GameInstanceStatus.Failed;
        }

        if (snapshot.Process is null)
            return GameInstanceStatus.Inactive;

        return snapshot.ProcessExitResult switch
        {
            null => GameInstanceStatus.Unknown,
            ProcessExitResult.Terminated => GameInstanceStatus.Terminated,
            ProcessExitResult.Failure => GameInstanceStatus.Crashed,
            _ => GameInstanceStatus.Exited
        };
    }
}