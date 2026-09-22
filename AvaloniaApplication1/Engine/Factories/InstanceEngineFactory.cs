using System;
using AvaloniaApplication1.Engine.Coordination;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine.Factories;

public class InstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartRequestFactory processStartRequestFactory, ILoggerFactory loggerFactory)
{
    public InstanceEngine Create(Guid id) => new(id, coordinator, processStartRequestFactory, loggerFactory);
}