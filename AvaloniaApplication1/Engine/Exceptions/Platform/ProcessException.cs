using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public abstract class ProcessException : PlatformException
{
    public ProcessException(string message) : base(message) { }
    
    public ProcessException(string message, Exception inner) : base(message, inner) { }
}