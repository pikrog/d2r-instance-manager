using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events.ProcessStop;

public sealed record ProcessStopped(ProcessCloseMode ProcessCloseMode) : Event;