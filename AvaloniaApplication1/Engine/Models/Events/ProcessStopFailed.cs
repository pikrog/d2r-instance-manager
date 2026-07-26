using System;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStopFailed : FailedEvent<ProcessStopError>
{
    public ProcessStopFailed(ProcessStopError error) : base(error)
    {
    }

    public ProcessStopFailed(Exception exception) : base(exception)
    {
    }
}