using AvaloniaApplication1.Engine.Coordination;

namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record LaunchLeaseGranted(LaunchLease Lease) : Event;