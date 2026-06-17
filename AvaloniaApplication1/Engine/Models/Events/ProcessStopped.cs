using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Models.Results;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessStopped(ProcessStopMode ProcessStopMode) : Event;