using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Instance.Models.Issues;
using AvaloniaApplication1.Selectable;
using AvaloniaApplication1.Semantic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Instance.ViewModels;

public partial class GameInstanceItemViewModel : SelectableViewModelBase
{
    // todo: runtime errors
    public Guid Id { get; set; }
    
    [ObservableProperty]
    public partial string Name { get; set; }
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusSemanticType), nameof(IsInProgress), nameof(CanBeStopped))]
    public partial GameInstanceStatus Status { get; set; }
    
    [ObservableProperty]
    public partial bool IsActive { get; set; }
    
    public ObservableCollection<GameInstanceIssue> Issues { get; }

    public bool IsInProgress =>
        Status is GameInstanceStatus.Authenticating
            or GameInstanceStatus.Starting
            or GameInstanceStatus.Stopping;

    public bool CanBeStopped => Status is not GameInstanceStatus.Stopping;

    public SemanticType StatusSemanticType =>
        Status switch
        {
            GameInstanceStatus.Inactive => SemanticType.Neutral,
            GameInstanceStatus.Authenticating => SemanticType.Info,
            GameInstanceStatus.Queued => SemanticType.Info,
            GameInstanceStatus.Starting => SemanticType.Info,
            GameInstanceStatus.Running => SemanticType.Success,
            GameInstanceStatus.Stopping => SemanticType.Info,
            GameInstanceStatus.Exited => SemanticType.Neutral,
            GameInstanceStatus.Terminated => SemanticType.Danger,
            GameInstanceStatus.Timeout => SemanticType.Danger,
            GameInstanceStatus.Failed => SemanticType.Danger,
            GameInstanceStatus.Crashed => SemanticType.Danger,
            GameInstanceStatus.Unknown => SemanticType.Danger,
            _ => throw new InvalidOperationException($"Unknown status {Status}")
        };

    public bool HasIssues => Issues.Any();
    
    public bool RequiresAttention => Issues.Any(i => i.RequiresAttention);
    
    public GameInstanceItemViewModel(Guid id, string name, GameInstanceStatus status, bool isActive, IReadOnlyList<GameInstanceIssue> issues)
    {
        Id = id;
        Name = name;
        Status = status;
        IsActive = isActive;
        Issues = new ObservableCollection<GameInstanceIssue>(issues);

        Issues.CollectionChanged += OnIssuesChanged;
    }

    private void OnIssuesChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(HasIssues));
        OnPropertyChanged(nameof(RequiresAttention));
    }
}