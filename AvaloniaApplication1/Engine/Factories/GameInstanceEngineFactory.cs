using System;
using AvaloniaApplication1.Engine.Coordination;

namespace AvaloniaApplication1.Engine.Factories;

public class GameInstanceEngineFactory(LaunchCoordinator coordinator, ProcessStartInfoFactory processStartInfoFactory)
{
    public GameInstanceEngine Create(Guid id) => new(id, coordinator, processStartInfoFactory);
}