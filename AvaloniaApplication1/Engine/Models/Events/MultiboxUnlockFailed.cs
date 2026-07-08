using System;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record MultiboxUnlockFailed : FailedEvent<RetryingMultiboxUnlockError>
{
    public MultiboxUnlockFailed(RetryingMultiboxUnlockError error) : base(error)
    {
    }

    public MultiboxUnlockFailed(Exception exception) : base(exception)
    {
    }
};
