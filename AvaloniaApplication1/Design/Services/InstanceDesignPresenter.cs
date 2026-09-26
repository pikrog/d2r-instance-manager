using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Design.Services;

public class InstanceDesignPresenter(InstanceService service) : IInstancePresenter
{
    #pragma warning disable CS0067
    public event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;
    #pragma warning restore CS0067
    
    public InstanceSummary Get(Guid instanceId) => service.GetSummary(instanceId);

    public IReadOnlyList<InstanceSummary> GetAll() => service.GetSummaries();
}