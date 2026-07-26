using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record MonitorProcessExit(ProcessManager ProcessManager, uint ForcefulExitCode) : Effect;