using System.Linq;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Engine.Models;

namespace AvaloniaApplication1.Bootstrap;

public class GameInstanceManagerBootstrapper(ConfigContext configContext, GameInstanceManager gameInstanceManager)
{
    public void Bootstrap()
    {
        var instanceIds = configContext.GetAllInstances().Select(i => i.Id).ToList();
        foreach (var id in instanceIds)
            gameInstanceManager.Register(id);
    }
}