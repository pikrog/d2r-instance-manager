using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Instance;

public interface IInstanceConfigProvider
{
    public event EventHandler<InstanceConfigChangedEventArgs> InstanceConfigChanged;
    public InstanceSnapshot GetConfigSnapshot(Guid id);
    public IReadOnlyList<InstanceSnapshot> GetConfigSnapshots();
}