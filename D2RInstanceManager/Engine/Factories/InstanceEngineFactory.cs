using System;
using D2RInstanceManager.Engine.Coordination;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Engine.Factories;

public class InstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartRequestFactory processStartRequestFactory, ILoggerFactory loggerFactory)
{
    public InstanceEngine Create(Guid id) => new(id, coordinator, processStartRequestFactory, loggerFactory);
}