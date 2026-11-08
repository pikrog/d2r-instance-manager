using System;
using D2RInstanceManager.Engine.Helpers.MultiboxUnlock;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record MultiboxUnlockFailed : FailedEvent<RetryingMultiboxUnlockError>
{
    public MultiboxUnlockFailed(RetryingMultiboxUnlockError error) : base(error)
    {
    }

    public MultiboxUnlockFailed(Exception exception) : base(exception)
    {
    }
};
