using System;

namespace AvaloniaApplication1.Engine.Models.Events;

public abstract record FailedEvent<TError> : ErrorEvent
{
    public TError? Error { get; init; }
    
    public Exception? Exception { get; init; }
    
    public bool IsKnown { get; init; }

    protected FailedEvent(TError error)
    {
        Error = error;
        Exception = null;
        IsKnown = true;
    }

    protected FailedEvent(Exception exception)
    {
        Error = default;
        Exception = exception;
        IsKnown = false;
    }
};