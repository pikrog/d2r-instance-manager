using System;

namespace AvaloniaApplication1.Engine.Exceptions;

public abstract class DomainException : Exception
{
    public DomainException(string message) : base(message) { }
    
    public DomainException(string message, Exception innerException) : base(message, innerException) { }
}