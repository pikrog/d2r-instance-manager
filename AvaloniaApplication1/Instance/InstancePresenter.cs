using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.HotKey.Coordination;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.EventArgs;
using AvaloniaApplication1.Instance.Models.Issues.HotKey;

namespace AvaloniaApplication1.Instance;

public class InstancePresenter : IInstancePresenter
{
    private readonly InstanceService _service;
    
    private readonly InstanceHotKeyBindingCoordinator _hotKeyBindingCoordinator;

    public InstancePresenter(InstanceService service, InstanceHotKeyBindingCoordinator hotKeyBindingCoordinator)
    {
        _service = service;
        _hotKeyBindingCoordinator = hotKeyBindingCoordinator;
        
        _service.InstanceConfigChanged += (_, args) => OnInstanceChanged(args.InstanceId);
        _service.InstanceStateChanged += (_, args) => OnInstanceChanged(args.Id);
        _hotKeyBindingCoordinator.BindingStateChanged += (_, args) => OnInstanceChanged(args.Command.InstanceId);
    }

    public event EventHandler<InstancePresentationChangedEventArgs>? InstancePresentationChanged;

    private void OnInstanceChanged(Guid instanceId) =>
        InstancePresentationChanged?.Invoke(
            this,
            new InstancePresentationChangedEventArgs(instanceId)
            );
    
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
            
            { State: HotKeyBindingState.NotConfigured } or { State: HotKeyBindingState.Registered } => 
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