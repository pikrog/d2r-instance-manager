using System;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Instance.Models;
using AvaloniaApplication1.Semantic;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Instance.ViewModels;

public partial class GameInstanceItemViewModel(Guid id, string name, GameInstanceStatus status, bool isActive) : ViewModelBase
{
    // todo:
    // requires attention reasons (cannot be launched)
    // warnings (fallback enabled and selected display is not available)
    // runtime errors?
    
    public Guid Id { get; set; } = id;
    
    [ObservableProperty]
    public partial string Name { get; set; } = name;
    
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(StatusSemanticType), nameof(IsInProgress), nameof(CanBeStopped))]
    public partial GameInstanceStatus Status { get; set; } = status;
    
    [ObservableProperty]
    public partial bool IsActive { get; set; } = isActive;

    public bool IsInProgress =>
        Status is GameInstanceStatus.Authenticating
            or GameInstanceStatus.Starting
            or GameInstanceStatus.Unlocking
            or GameInstanceStatus.Stopping;

    public bool CanBeStopped => Status is not GameInstanceStatus.Stopping;

    public SemanticType StatusSemanticType =>
        Status switch
        {
            GameInstanceStatus.Inactive => SemanticType.Neutral,
            GameInstanceStatus.Authenticating => SemanticType.Info,
            GameInstanceStatus.QueuedForStart => SemanticType.Info,
            GameInstanceStatus.Starting => SemanticType.Info,
            GameInstanceStatus.Unlocking => SemanticType.Info,
            GameInstanceStatus.Running => SemanticType.Success,
            GameInstanceStatus.Stopping => SemanticType.Info,
            GameInstanceStatus.Exited => SemanticType.Neutral,
            GameInstanceStatus.ExitedPrematurely => SemanticType.Danger,
            GameInstanceStatus.Terminated => SemanticType.Danger,
            GameInstanceStatus.Timeout => SemanticType.Danger,
            GameInstanceStatus.Failed => SemanticType.Danger,
            GameInstanceStatus.Unknown => SemanticType.Danger,
            _ => throw new InvalidOperationException($"Unknown status {Status}")
        };

    public bool RequiresAttention => false;
}