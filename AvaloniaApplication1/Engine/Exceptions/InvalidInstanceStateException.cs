using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class InvalidInstanceStateException(string message) : InstanceException(message)
{
    public static InvalidInstanceStateException CreateDeleteWhileRunning(Guid id) => 
        new($"Cannot delete game instance {id} while it is running");
    
    public static InvalidInstanceStateException CreateEditWhileRunning(Guid id) => 
        new($"Cannot edit game instance {id} while it is running");
    
    public static InvalidInstanceStateException CreateStartWhileRunning(Guid id) => 
        new($"Cannot start game instance {id} while it is running");
}