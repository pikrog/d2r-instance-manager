using System;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.StateMachine;
using AvaloniaApplication1.Instance.Models;

namespace AvaloniaApplication1.Instance;

public static class InstanceStatusMapper
{
    public static InstanceStatus Map(RuntimeSnapshot snapshot) =>
        snapshot.State switch
        {
            State.Inactive => ResolveInactiveStatus(snapshot),
            State.Authenticating => InstanceStatus.Authenticating,
            State.WaitingForStart => InstanceStatus.Queued,
            State.Starting or State.WaitingForUnlock => InstanceStatus.Starting,
            State.Running => InstanceStatus.Running,
            State.Stopping => InstanceStatus.Stopping,
            State.Shutdown => InstanceStatus.Inactive,
            _ => throw new InvalidOperationException($"Unknown state {snapshot.State}")
        };

    private static InstanceStatus ResolveInactiveStatus(RuntimeSnapshot snapshot)
    {
        if (snapshot.Errors.Length > 0)
        {
            if (snapshot.Errors[0] is MultiboxUnlockFailed { IsKnown: true, Error: RetryingMultiboxUnlockError.Timeout } 
                || snapshot.Errors[0] is ProcessStopFailed { IsKnown: true, Error: ProcessStopTimeout })
                return InstanceStatus.Timeout;
            return InstanceStatus.Failed;
        }

        if (snapshot.Process is null)
            return InstanceStatus.Inactive;

        return snapshot.ProcessExitResult switch
        {
            null => InstanceStatus.Unknown,
            ProcessExitResult.Terminated => InstanceStatus.Terminated,
            ProcessExitResult.Failure => InstanceStatus.Crashed,
            _ => InstanceStatus.Exited
        };
    }
}