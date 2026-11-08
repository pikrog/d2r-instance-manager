using System;
using System.Collections.Generic;
using System.Linq;
using D2RInstanceManager.HotKey.Coordination;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.EventArgs;
using D2RInstanceManager.Instance.Models.Issues.HotKey;

namespace D2RInstanceManager.Instance;

public class InstancePresenter : IInstancePresenter
{
    private readonly InstanceService _service;
    
    private readonly InstanceHotKeyBindingCoordinator _hotKeyBindingCoordinator;
    
    private readonly HashSet<Guid> _knownInstanceIds;

    public InstancePresenter(InstanceService service, InstanceHotKeyBindingCoordinator hotKeyBindingCoordinator)
    {
        _service = service;
        _hotKeyBindingCoordinator = hotKeyBindingCoordinator;
        
        _knownInstanceIds = _service.GetConfigSnapshots()
            .Select(x => x.Id)
            .ToHashSet();
        
        _service.InstanceConfigChanged += OnInstanceConfigChanged;
        _service.InstanceStateChanged += (_, args) => OnInstanceChanged(args.Id);
        _hotKeyBindingCoordinator.BindingStateChanged += (_, args) => OnInstanceChanged(args.Command.InstanceId);
    }

    public event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;

    private void OnInstanceConfigChanged(object? sender, InstanceConfigChangedEventArgs e)
    {
        if (e.NewSnapshot is null)
        {
            _knownInstanceIds.Remove(e.InstanceId);
            InstancePresentationChanged?.Invoke(
                this, 
                InstancePresentationChangedEventArgs.Removed(e.InstanceId));
            return;
        }
        
        _knownInstanceIds.Add(e.InstanceId);
        OnInstanceChanged(e.InstanceId);
    }

    private void OnInstanceChanged(Guid instanceId)
    {
        if (!_knownInstanceIds.Contains(instanceId))
            return;
        
        InstancePresentationChanged?.Invoke(
            this, 
            InstancePresentationChangedEventArgs.Updated(instanceId));
    }

    private InstanceSummary ApplyHotKeyIssueIfNeeded(InstanceSummary summary)
    {
        var instanceId = summary.Id;
        var binding = _hotKeyBindingCoordinator.GetBindingSummary(instanceId);
        var keyCombination = binding.KeyCombination;

        HotKeyIssue? issue = binding switch
        {
            { State: HotKeyBindingState.ExternalConflict } => 
                new ExternalHotKeyConflict(keyCombination),
            
            { State: HotKeyBindingState.InternalConflict } => 
                new InternalHotKeyConflict(keyCombination),
            
            { State: HotKeyBindingState.UnknownError, Error: { } error } =>
                new FailedHotKeyRegistration(keyCombination, error.Exception),
            
            { State: HotKeyBindingState.NotConfigured } 
                or { State: HotKeyBindingState.Registered }
                or { State: HotKeyBindingState.Pending } => 
                null,
  
            _ => throw new InvalidOperationException($"Invalid hot key binding summary: {binding}")
        };
        
        if (issue is null)
            return summary;
        
        return summary with
        {
            Issues = summary.Issues.Append(issue).ToList()
        };
    }
    
    public InstanceSummary Get(Guid instanceId)
    {
        var summary = _service.GetSummary(instanceId);
        return ApplyHotKeyIssueIfNeeded(summary);
    }
    
    public IReadOnlyList<InstanceSummary> GetAll() => _service.GetSummaries().Select(ApplyHotKeyIssueIfNeeded).ToList();
}