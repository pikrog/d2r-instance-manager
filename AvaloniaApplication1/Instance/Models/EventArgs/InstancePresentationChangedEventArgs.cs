using System;

namespace AvaloniaApplication1.Instance.Models.EventArgs;

public class InstancePresentationChangedEventArgs(Guid instanceId, bool isRemoved) : System.EventArgs
{
    public Guid InstanceId { get; } = instanceId;

    public bool IsRemoved { get; } = isRemoved;
    
    public static InstancePresentationChangedEventArgs Updated(Guid instanceId) => new(instanceId, false);
    
    public static InstancePresentationChangedEventArgs Removed(Guid instanceId) => new(instanceId, true);
}