using System;
using AvaloniaApplication1.Engine.Exceptions;

namespace AvaloniaApplication1.Engine.Platform.Exceptions;

public abstract class PlatformException : DomainException
{
    public PlatformException(string message) : base(message) { }
    
    public PlatformException(string message, Exception inner) : base(message, inner) { }
}