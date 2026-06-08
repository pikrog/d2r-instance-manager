using AvaloniaApplication1.Engine.Mappers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Factories;

public class ProcessStartInfoFactory(ArgumentsFactory argumentsFactory)
{
    public ProcessStartInfo Create(LaunchContext context)
    {
        var argumentsContext = ArgumentsContextMapper.Map(context);
        return new ProcessStartInfo
        {
            ExecutablePath = context.ExecutablePath,
            Arguments = argumentsFactory.CreateString(argumentsContext),
            DisplayHandle = Display.GetDisplayHandle(context.DisplayId),
        };
    }
}