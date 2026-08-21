using System;

namespace AvaloniaApplication1.Instance.Models;

public class InstanceStateChangedEventArgs(Guid instanceId) : EventArgs
{
    public Guid InstanceId { get; } = instanceId;
}