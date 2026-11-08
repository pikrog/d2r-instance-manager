using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using D2RInstanceManager.Instance.Models;
using D2RInstanceManager.Instance.Models.Issues;
using D2RInstanceManager.Selectable;
using D2RInstanceManager.Semantic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Instance.ViewModels;

public partial class InstanceItemViewModel : SelectableObservable
{
    // todo: runtime errors
    public Guid Id { get; set; }
    
    [ObservableProperty]
    public partial string Name { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusSemanticType), nameof(IsInProgress), nameof(CanStop))]
    public partial InstanceStatus Status { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanLaunch), nameof(CanStop), nameof(CanEdit), nameof(CanDelete), nameof(CanShow))]
    public partial bool IsActive { get; set; }
    
    public ObservableCollection<InstanceIssue> Issues { get; }

    public bool IsInProgress =>
        Status is InstanceStatus.Authenticating
            or InstanceStatus.Starting
            or InstanceStatus.Stopping;
    

    public SemanticType StatusSemanticType =>
        Status switch
        {
            InstanceStatus.Inactive => SemanticType.Neutral,
            InstanceStatus.Authenticating => SemanticType.Info,
            InstanceStatus.Queued => SemanticType.Info,
            InstanceStatus.Starting => SemanticType.Info,
            InstanceStatus.Running => SemanticType.Success,
            InstanceStatus.Stopping => SemanticType.Info,
            InstanceStatus.Exited => SemanticType.Neutral,
            InstanceStatus.Terminated => SemanticType.Danger,
            InstanceStatus.Timeout => SemanticType.Danger,
            InstanceStatus.Failed => SemanticType.Danger,
            InstanceStatus.Crashed => SemanticType.Danger,
            InstanceStatus.Unknown => SemanticType.Danger,
            _ => throw new InvalidOperationException($"Unknown status {Status}")
        };
    
    public bool HasIssues => Issues.Any();
    
    public bool RequiresAttention => Issues.Any(i => i.RequiresAttention);
    
    public SemanticType? IssuesSemanticType
    {
        get
        {
            if (IsActive || !HasIssues)
                return null;

            return RequiresAttention
                ? SemanticType.Danger
                : SemanticType.Warning;
        }
    }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(CanEdit), nameof(CanDelete))]
    public partial bool IsLaunchPending { get; set; }
    
    public bool CanLaunch => !RequiresAttention && !IsActive; 
    
    public bool CanStop => IsActive && Status is not InstanceStatus.Stopping;
    
    public bool CanEdit => !IsActive && !IsLaunchPending;
    
    public bool CanDelete => !IsActive && !IsLaunchPending;
    
    public bool CanShow => IsActive;
    
    public InstanceItemViewModel(Guid id, string name, InstanceStatus status, bool isActive, IReadOnlyList<InstanceIssue> issues)
    {
        Id = id;
        Name = name;
        Status = status;
        IsActive = isActive;
        Issues = new ObservableCollection<InstanceIssue>(issues);

        Issues.CollectionChanged += OnIssuesChanged;
    }
    
    private void OnIssuesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasIssues));
        OnPropertyChanged(nameof(RequiresAttention));
        OnPropertyChanged(nameof(IssuesSemanticType));
        
        OnPropertyChanged(nameof(CanLaunch));
    }
}