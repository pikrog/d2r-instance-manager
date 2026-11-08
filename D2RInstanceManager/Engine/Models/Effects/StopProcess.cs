using D2RInstanceManager.Engine.Helpers.ProcessStop;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record StopProcess(ProcessManager ProcessManager, ProcessStopPolicies Policies) : Effect;