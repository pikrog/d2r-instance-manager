using System;
using AvaloniaApplication1.Engine.Mappers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Engine.Providers;

namespace AvaloniaApplication1.Engine.Factories;

public class ProcessStartInfoFactory(ArgumentsFactory argumentsFactory)
{
    public ProcessStartInfo Create(InstanceLaunchContext context)
    {
        var argumentsContext = ArgumentsContextMapper.Map(context);
        var display = Display.GetById(context.DisplayId) 
                      ?? throw new InvalidOperationException($"Display with id {context.DisplayId} not found.");
        
        return new ProcessStartInfo
        {
            FileName = context.ExecutablePath,
            Arguments = argumentsFactory.CreateString(argumentsContext),
            DisplayHandle = display.Handle,
        };
    }
}