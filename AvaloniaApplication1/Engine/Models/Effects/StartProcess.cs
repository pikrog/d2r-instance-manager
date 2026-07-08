using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record StartProcess(ProcessStartInfo ProcessStartInfo) : Effect;