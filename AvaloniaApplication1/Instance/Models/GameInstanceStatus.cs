namespace AvaloniaApplication1.Instance.Models;

public enum GameInstanceStatus
{
    Inactive,
    
    Authenticating,
    
    QueuedForStart,
    Starting,
    Unlocking,
    Running,

    Stopping,

    Exited,
    ExitedPrematurely,
    Terminated,
    Timeout,
    Failed,

    Unknown,
}