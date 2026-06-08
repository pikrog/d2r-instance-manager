using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Contexts;
using AvaloniaApplication1.Engine.Models.Contexts.Launch;

namespace AvaloniaApplication1.Engine;

public interface IGameInstanceEngineControl
{
    Task StartAsync(LaunchContext launchContext);
    Task StopAsync();
    Task ShutdownAsync();
}