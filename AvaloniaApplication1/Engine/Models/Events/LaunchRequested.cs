using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record LaunchRequested(AuthenticationContext AuthenticationContext, ProcessStartInfo ProcessStartInfo, EnginePolicies EnginePolicies) : Event;