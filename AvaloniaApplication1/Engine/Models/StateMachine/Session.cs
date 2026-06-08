using System.Collections.Immutable;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public record Session
{
    public State State { get; init; } = State.Inactive;
    public CriticalSectionLease? Lease { get; init; }
    public RetryPolicy? MultiboxUnlockRetryPolicy { get; init; }
    public RetryPolicy? ProcessStopRetryPolicy { get; init; }
    public ProcessStartInfo? ProcessStartInfo { get; init; }
    public Process? Process { get; init; }
    public CleanupState CleanupState { get; init; } = new();
    public ImmutableArray<ErrorEvent> ErrorEvents = [];
    
    public static Session SetLeaseRequested(Session s) => 
        s with { CleanupState = s.CleanupState with { IsLeaseRequested = true } };
    
    public static Session SetProcessStartRequested(Session s) =>
        s with { CleanupState = s.CleanupState with { IsProcessStartRequested = true } };
    
    public static Session SetLeaseReleased(Session s) => 
        s with { CleanupState = s.CleanupState with { IsLeaseCleanedUp = true } };
    
    public static Session SetProcessFinished(Session s) => 
        s with { CleanupState = s.CleanupState with { IsProcessCleanedUp = true } };
}