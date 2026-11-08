using System;
using System.Collections.Generic;
using D2RInstanceManager.Instance;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;

namespace D2RInstanceManager.Design.Services;

public class InstanceDesignPresenter(InstanceService service) : IInstancePresenter
{
    #pragma warning disable CS0067
    public event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;
    #pragma warning restore CS0067
    
    public InstanceSummary Get(Guid instanceId) => service.GetSummary(instanceId);

    public IReadOnlyList<InstanceSummary> GetAll() => service.GetSummaries();
}