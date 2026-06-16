using System;
using AvaloniaApplication1.Engine.Models.Errors;

namespace AvaloniaApplication1.Engine.Models.Events.MultiboxUnlock;

public sealed record MultiboxUnlockFailed : FailedEvent<RetryingMultiboxUnlockError>
{
    public MultiboxUnlockFailed(RetryingMultiboxUnlockError error) : base(error)
    {
    }

    public MultiboxUnlockFailed(Exception exception) : base(exception)
    {
    }
};
