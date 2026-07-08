using System;
using AvaloniaApplication1.Instance.Models;

namespace AvaloniaApplication1.Instance;

public static class GameInstanceStatusPresentation
{
    public static string GetDisplayText(GameInstanceStatus status) =>
        status switch
        {
            GameInstanceStatus.Inactive => "Inactive",
            GameInstanceStatus.Authenticating => "Authenticating",
            GameInstanceStatus.QueuedForStart => "Queued for start",
            GameInstanceStatus.Starting => "Starting",
            GameInstanceStatus.Unlocking => "Unlocking",
            GameInstanceStatus.Running => "Running",
            GameInstanceStatus.Stopping => "Stopping",
            GameInstanceStatus.Exited => "Exited",
            GameInstanceStatus.ExitedPrematurely => "Exited prematurely",
            GameInstanceStatus.Terminated => "Terminated",
            GameInstanceStatus.Timeout => "Timeout",
            GameInstanceStatus.Failed => "Failed",
            GameInstanceStatus.Unknown => "Unknown",
            _ => throw new InvalidOperationException($"Unknown status {status}")
        };
}