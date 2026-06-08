namespace AvaloniaApplication1.Engine.Models.Events.ProcessExit;

public sealed record ProcessExited(int ExitCode) : Event;