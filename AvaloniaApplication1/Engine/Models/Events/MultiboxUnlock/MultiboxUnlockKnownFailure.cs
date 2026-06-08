namespace AvaloniaApplication1.Engine.Models.Events.MultiboxUnlock;

public sealed record MultiboxUnlockKnownFailure(MultiboxUnlockFailureReason Reason) : MultiboxUnlockFailed;