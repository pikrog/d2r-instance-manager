using System;

namespace AvaloniaApplication1.Instance.Models.EventArgs;

public class InstanceStateChangedEventArgs(Guid instanceId) : System.EventArgs
{
    public Guid InstanceId { get; } = instanceId;
}