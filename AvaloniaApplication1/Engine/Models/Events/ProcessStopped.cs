using AvaloniaApplication1.Engine.Helpers.ProcessStop;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStopped(ProcessStopMode ProcessStopMode) : Event;