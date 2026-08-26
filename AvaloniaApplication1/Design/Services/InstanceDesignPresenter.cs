using System;
using System.Collections.Generic;
using AvaloniaApplication1.Instance;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Design.Services;

public class InstanceDesignPresenter : IInstancePresenter
{
    private readonly InstanceService _service;

    public InstanceDesignPresenter(InstanceService service)
    {
        _service = service;
        
        _service.InstanceConfigChanged += (_, args) => InstancePresentationChanged?.Invoke(this, new InstancePresentationChangedEventArgs(args.InstanceId));
    }

    public event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;
    
    public InstanceSummary Get(Guid instanceId) => _service.GetSummary(instanceId);

    public IReadOnlyList<InstanceSummary> GetAll() => _service.GetSummaries();
}