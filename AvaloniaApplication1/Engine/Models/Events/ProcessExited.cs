using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record ProcessExited(uint? ExitCode = null, ProcessError? Error = null) : Event;