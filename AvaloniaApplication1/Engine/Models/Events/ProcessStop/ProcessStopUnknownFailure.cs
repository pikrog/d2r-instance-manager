using System;

namespace AvaloniaApplication1.Engine.Models.Events.ProcessStop;

public sealed record ProcessStopUnknownFailure(Exception Exception) : ProcessStopFailed;