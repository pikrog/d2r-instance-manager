using System;

namespace D2RInstanceManager.Instance.Models.EventArgs;

public class InstanceStateChangedEventArgs(Guid instanceId) : System.EventArgs
{
    public Guid InstanceId { get; } = instanceId;
}