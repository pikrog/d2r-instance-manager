namespace AvaloniaApplication1.Instance.Models;

public enum GameInstanceStatus
{
    Inactive,
    
    Authenticating,
    
    Queued,
    Starting,
    Running,

    Stopping,

    Exited,
    Terminated,
    Timeout,
    Failed,
    Crashed,

    Unknown,
}