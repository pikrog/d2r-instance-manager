using AvaloniaApplication1.Engine.Models.Contexts;

namespace AvaloniaApplication1.Engine.Models.Effects;

public sealed record StartProcess(ProcessStartContext ProcessStartContext) : Effect;