using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record StartRequested(AuthenticationContext AuthenticationContext, ProcessStartInfo ProcessStartInfo) : Event;