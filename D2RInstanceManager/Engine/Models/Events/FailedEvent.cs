using System;
using System.Diagnostics.CodeAnalysis;
using D2RInstanceManager.Common;

namespace D2RInstanceManager.Engine.Models.Events;

public abstract record FailedEvent<TError> : ErrorEvent
{
    public TError? Error { get; init; }
    
    public Exception? Exception { get; init; }
    
    [MemberNotNullWhen(true, nameof(Error))]
    [MemberNotNullWhen(false, nameof(Exception))]
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

    protected virtual string Summary => GetType().Name.ToSentenceCase();

    protected abstract string GetKnownErrorMessage(TError error);

    public override string Message => $"{Summary}. {(IsKnown ? GetKnownErrorMessage(Error) : Exception.Message)}";
};