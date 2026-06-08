using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public abstract class PlatformException : DomainException
{
    public PlatformException(string message) : base(message) { }
    
    public PlatformException(string message, Exception inner) : base(message, inner) { }
}