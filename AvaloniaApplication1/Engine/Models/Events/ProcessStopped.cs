using D2RInstanceManager.Engine.Helpers.ProcessStop;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record ProcessStopped(ProcessStopMode ProcessStopMode) : Event;