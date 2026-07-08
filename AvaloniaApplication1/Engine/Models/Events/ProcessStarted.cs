using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStarted(ProcessManager ProcessManager) : Event;