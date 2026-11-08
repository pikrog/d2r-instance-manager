using System;
using System.Collections.Generic;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;

namespace D2RInstanceManager.Instance;

public interface IInstancePresenter
{
    event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;
    InstanceSummary Get(Guid instanceId);
    IReadOnlyList<InstanceSummary> GetAll();
}