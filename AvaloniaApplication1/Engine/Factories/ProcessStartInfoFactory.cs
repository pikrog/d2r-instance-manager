using System;
using AvaloniaApplication1.Engine.Mappers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Engine.Providers;

namespace AvaloniaApplication1.Engine.Factories;

public class ProcessStartInfoFactory(ArgumentsFactory argumentsFactory, DisplayResolver displayResolver)
{
    public ProcessStartInfo Create(LaunchContext context)
    {
        var argumentsContext = ArgumentsContextMapper.Map(context);
        var display = displayResolver.GetByIndex(context.DisplayId);
        
        return new ProcessStartInfo
        {
            FileName = context.ExecutablePath,
            Arguments = argumentsFactory.CreateString(argumentsContext),
            DisplayHandle = display.Handle,
        };
    }
}