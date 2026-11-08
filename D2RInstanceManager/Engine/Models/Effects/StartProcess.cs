using D2RInstanceManager.Engine.Models.Contexts;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record StartProcess(ProcessStartContext ProcessStartContext) : Effect;