using System;
using D2RInstanceManager.Engine.Helpers.ProcessStop.Error;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record ProcessStopFailed : FailedEvent<ProcessStopError>
{
    public ProcessStopFailed(ProcessStopError error) : base(error)
    {
    }

    public ProcessStopFailed(Exception exception) : base(exception)
    {
    }
}