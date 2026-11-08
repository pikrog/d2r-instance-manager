using D2RInstanceManager.Engine.Coordination;

namespace D2RInstanceManager.Engine.Models.Effects;

public sealed record ReleaseLaunchLease(LaunchLease Lease) : Effect;