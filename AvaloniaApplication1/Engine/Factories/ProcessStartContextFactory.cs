using AvaloniaApplication1.Engine.Mappers;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Factories;

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