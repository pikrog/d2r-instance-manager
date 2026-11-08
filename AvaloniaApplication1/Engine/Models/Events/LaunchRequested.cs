using D2RInstanceManager.Engine.Models.Contexts;
using D2RInstanceManager.Engine.Models.Contexts.Launch;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record LaunchRequested(
    AuthenticationContext AuthenticationContext,
    ProcessStartContext ProcessStartContext,
    EnginePolicies EnginePolicies) : Event;