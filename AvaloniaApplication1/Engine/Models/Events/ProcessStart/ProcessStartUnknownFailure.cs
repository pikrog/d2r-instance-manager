using System;

namespace AvaloniaApplication1.Engine.Models.Events.ProcessStart;

public sealed record ProcessStartUnknownFailure(Exception Exception) : ProcessStartFailed;