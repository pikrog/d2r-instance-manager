using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvaloniaApplication1.Models;

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