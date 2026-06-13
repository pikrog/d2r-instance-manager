using AvaloniaApplication1.Engine.Exceptions;

namespace AvaloniaApplication1.Engine.Models.Events.MultiboxUnlock;

public sealed record MultiboxUnlockKnownFailure(MultiboxUnlockException Exception) : MultiboxUnlockFailed;