namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record CleanupState
{
    public bool IsLeaseRequested { get; init; }
    public bool IsProcessStartRequested { get; init; }
    public bool IsLeaseCleanedUp { get; init; }
    public bool IsProcessCleanedUp { get; init; }

    public bool IsCleanupComplete =>
        (!IsLeaseRequested || IsLeaseCleanedUp) && (!IsProcessStartRequested || IsProcessCleanedUp);
}