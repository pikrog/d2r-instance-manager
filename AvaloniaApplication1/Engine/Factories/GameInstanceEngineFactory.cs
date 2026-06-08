using System;

namespace AvaloniaApplication1.Engine.Factories;

public class GameInstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartInfoFactory processStartInfoFactory)
{
    public GameInstanceEngine Create(Guid id) => new(id, coordinator, processStartInfoFactory);
}