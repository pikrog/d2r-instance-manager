using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessExited(ProcessExitResult Result, ProcessError? Error = null) : Event;