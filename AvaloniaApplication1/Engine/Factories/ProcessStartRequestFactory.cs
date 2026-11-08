using D2RInstanceManager.Engine.CommandLine;
using D2RInstanceManager.Engine.Models.Contexts;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Factories;

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