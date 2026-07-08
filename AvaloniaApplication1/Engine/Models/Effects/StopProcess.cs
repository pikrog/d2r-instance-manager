using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record StopProcess(ProcessManager ProcessManager, ProcessStopPolicies Policies) : Effect;