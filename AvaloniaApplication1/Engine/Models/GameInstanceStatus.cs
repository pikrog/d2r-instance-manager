namespace AvaloniaApplication1.Engine.Models;

// todo: this is a service layer enum
public enum GameInstanceStatus
{
    Inactive,
    
    Authenticating,
    
    QueuedForStart,
    Starting,
    Unlocking,
    Running,

    Stopping,
    Closing, // ?

    Exited,
    ExitedPrematurely,
    Terminated,
    Timeout,
    Failed,

    Unknown,
}