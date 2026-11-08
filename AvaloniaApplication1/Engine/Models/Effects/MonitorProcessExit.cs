using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record MonitorProcessExit(ProcessManager ProcessManager, uint ForcefulExitCode) : Effect;