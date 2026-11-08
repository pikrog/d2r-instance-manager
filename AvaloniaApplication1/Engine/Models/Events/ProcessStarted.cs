using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record ProcessStarted(ProcessManager ProcessManager) : Event;