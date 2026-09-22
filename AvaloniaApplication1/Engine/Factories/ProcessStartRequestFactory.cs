using AvaloniaApplication1.Engine.CommandLine;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Factories;

public class ProcessStartRequestFactory(IArgumentStringBuilder argumentStringBuilder)
{
    public ProcessStartRequest Create(ProcessStartContext startContext) => new()
        {
            FileName = startContext.ExecutablePath,
            Arguments = argumentStringBuilder.Build(startContext.Arguments),
            DisplayHandle = startContext.DisplayHandle,
        };

    public ProcessStartRequest CreateRedacted(ProcessStartContext startContext) => new()
        {
            FileName = startContext.ExecutablePath,
            Arguments = argumentStringBuilder.BuildRedacted(startContext.Arguments),
            DisplayHandle = startContext.DisplayHandle,
        };
}