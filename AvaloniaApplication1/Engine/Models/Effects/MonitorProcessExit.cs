using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record MonitorProcessExit(Process Process) : Effect;