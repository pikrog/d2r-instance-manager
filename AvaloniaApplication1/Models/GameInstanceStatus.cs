using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace AvaloniaApplication1.Models;

public enum GameInstanceStatus
{
    Inactive,
    
    Authenticating,
    
    [Display(Name = "Queued for start")] QueuedForStart,
    Starting,
    Unlocking,
    Running,

    Stopping,

    Exited,
    [Display(Name = "Exited prematurely")] ExitedPrematurely,
    Terminated,
    Timeout,
    Failed,

    Unknown,
}