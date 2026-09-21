using System;
using AvaloniaApplication1.Engine.Coordination;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine.Factories;

public class InstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartInfoFactory processStartInfoFactory, ILoggerFactory loggerFactory)
{
    public InstanceEngine Create(Guid id) => new(id, coordinator, processStartInfoFactory, loggerFactory);
}