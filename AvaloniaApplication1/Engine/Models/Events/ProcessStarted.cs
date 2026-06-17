using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStarted(Process Process) : Event;