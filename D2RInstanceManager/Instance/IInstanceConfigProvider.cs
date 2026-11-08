using System;
using System.Collections.Generic;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;

namespace D2RInstanceManager.Instance;

public interface IInstanceConfigProvider
{
    public event EventHandler<InstanceConfigChangedEventArgs> InstanceConfigChanged;
    public InstanceSnapshot GetConfigSnapshot(Guid id);
    public IReadOnlyList<InstanceSnapshot> GetConfigSnapshots();
}