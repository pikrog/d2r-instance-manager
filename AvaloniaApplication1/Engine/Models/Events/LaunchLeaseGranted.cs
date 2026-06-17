namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record LaunchLeaseGranted(CriticalSectionLease Lease) : Event;