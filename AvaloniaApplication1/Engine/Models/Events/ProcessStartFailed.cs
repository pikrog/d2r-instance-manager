using System;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStartFailed : FailedEvent<ProcessError>
{
    public ProcessStartFailed(ProcessError error) : base(error)
    {
    }
    
    public ProcessStartFailed(Exception exception) : base(exception)
    {
    }
}