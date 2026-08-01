namespace AvaloniaApplication1.Instance.Models;

public enum InstanceStatus
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