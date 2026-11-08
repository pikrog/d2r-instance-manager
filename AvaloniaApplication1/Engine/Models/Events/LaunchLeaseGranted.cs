using D2RInstanceManager.Engine.Coordination;

namespace D2RInstanceManager.Engine.Models.Events;

public sealed record LaunchLeaseGranted(LaunchLease Lease) : Event;