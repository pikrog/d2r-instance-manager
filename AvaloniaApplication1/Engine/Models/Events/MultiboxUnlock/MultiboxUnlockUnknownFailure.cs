using System;

namespace AvaloniaApplication1.Engine.Models.Events.MultiboxUnlock;

public sealed record MultiboxUnlockUnknownFailure(Exception Exception) : MultiboxUnlockFailed;