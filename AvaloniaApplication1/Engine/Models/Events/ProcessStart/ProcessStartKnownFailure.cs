namespace AvaloniaApplication1.Engine.Models.Events.ProcessStart;

public sealed record ProcessStartKnownFailure(ProcessStartFailureReason Reason) : ProcessStartFailed;