using System;

namespace AvaloniaApplication1.Instance.Models.EventArgs;

public class InstanceConfigChangedEventArgs(Guid instanceId, InstanceSnapshot? oldSnapshot = null, InstanceSnapshot? newSnapshot = null)
    : System.EventArgs
{
    public Guid InstanceId { get; } = instanceId;

    public InstanceSnapshot? OldSnapshot { get; } = oldSnapshot;

    public InstanceSnapshot? NewSnapshot { get; } = newSnapshot;
}