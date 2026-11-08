using D2RInstanceManager.Engine.Mappers;
using D2RInstanceManager.Engine.Models.Contexts;
using D2RInstanceManager.Engine.Models.Contexts.Launch;
using D2RInstanceManager.Engine.Platform;

namespace D2RInstanceManager.Engine.Factories;

public static class ProcessStartContextFactory
{
    public static ProcessStartContext Create(InstanceLaunchContext context)
    {
        var argumentsContext = ArgumentsContextMapper.Map(context);
        var arguments = ArgumentsFactory.Create(argumentsContext);
        
        var display = DisplayInfo.GetById(context.DisplayId);
        
        return new ProcessStartContext(context.ExecutablePath, arguments, display?.Handle);
    }
}