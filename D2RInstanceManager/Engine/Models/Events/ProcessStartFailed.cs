using System;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record ProcessStartFailed : FailedEvent<ProcessError>
{
    public ProcessStartFailed(ProcessError error) : base(error)
    {
    }
    
    public ProcessStartFailed(Exception exception) : base(exception)
    {
    }

    protected override string GetKnownErrorMessage(ProcessError error) => error.Message;
}