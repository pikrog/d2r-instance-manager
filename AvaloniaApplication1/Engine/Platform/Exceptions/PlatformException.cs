using System;
using D2RInstanceManager.Engine.Exceptions;

namespace D2RInstanceManager.Engine.Platform.Exceptions;

public abstract class PlatformException : DomainException
{
    public PlatformException(string message) : base(message) { }
    
    public PlatformException(string message, Exception inner) : base(message, inner) { }
}