using System;
using AvaloniaApplication1.Engine.Coordination;

namespace AvaloniaApplication1.Engine.Factories;

public class InstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartInfoFactory processStartInfoFactory)
{
    public InstanceEngine Create(Guid id) => new(id, coordinator, processStartInfoFactory);
}