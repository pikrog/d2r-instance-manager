namespace AvaloniaApplication1.Engine.Models.Events.ProcessStop;

public sealed record ProcessStopKnownFailure(ProcessStopFailureReason Reason) : ProcessStopFailed;