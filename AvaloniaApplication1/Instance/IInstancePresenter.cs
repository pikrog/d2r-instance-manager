using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Instance;

public interface IInstancePresenter
{
    event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;
    InstanceSummary Get(Guid instanceId);
    IReadOnlyList<InstanceSummary> GetAll();
}