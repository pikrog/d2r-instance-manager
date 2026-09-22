using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record LaunchRequested(
    AuthenticationContext AuthenticationContext,
    ProcessStartContext ProcessStartContext,
    EnginePolicies EnginePolicies) : Event;