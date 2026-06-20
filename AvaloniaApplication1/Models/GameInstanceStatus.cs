using System.ComponentModel;

namespace AvaloniaApplication1.Models;

public enum GameInstanceStatus
{
    Inactive,
    
    Authenticating,
    
    [Description("Queued for start")] QueuedForStart,
    Starting,
    Unlocking,
    Running,

    Stopping,

    Exited,
    [Description("Exited prematurely")] ExitedPrematurely,
    Terminated,
    Timeout,
    Failed,

    Unknown,
}