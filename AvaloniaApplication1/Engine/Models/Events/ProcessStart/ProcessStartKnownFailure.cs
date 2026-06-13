using AvaloniaApplication1.Engine.Exceptions.Platform;

namespace AvaloniaApplication1.Engine.Models.Events.ProcessStart;

public sealed record ProcessStartKnownFailure(ProcessStartException Exception) : ProcessStartFailed;