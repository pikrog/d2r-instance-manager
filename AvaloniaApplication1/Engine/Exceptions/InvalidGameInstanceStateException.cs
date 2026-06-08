using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public class InvalidGameInstanceStateException(string message) : GameInstanceException(message)
{
    public static InvalidGameInstanceStateException CreateDeleteWhileRunning(Guid id) => 
        new($"Cannot delete game instance {id} while it is running");
    
    public static InvalidGameInstanceStateException CreateEditWhileRunning(Guid id) => 
        new($"Cannot edit game instance {id} while it is running");
    
    public static InvalidGameInstanceStateException CreateStartWhileRunning(Guid id) => 
        new($"Cannot start game instance {id} while it is running");
}