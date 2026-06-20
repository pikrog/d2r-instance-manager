using System;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

[Flags]
public enum CleanupItem
{
    None = 0,
    LaunchLease = 1,
    Process = 2,
}

public sealed record CleanupState(CleanupItem Pending = CleanupItem.None)
{
    public bool IsCleanupComplete => Pending == CleanupItem.None;

    public CleanupState Require(CleanupItem item) =>
        this with { Pending = Pending | item };

    public CleanupState Complete(CleanupItem item) =>
        this with { Pending = Pending & ~item };
}