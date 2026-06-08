using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record StartRequested(LaunchContext LaunchContext) : Event;