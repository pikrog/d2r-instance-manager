using System.Linq;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;

namespace AvaloniaApplication1.Bootstrap;

public class InstanceManagerBootstrapper(ConfigContext configContext, InstanceManager instanceManager)
{
    public void Bootstrap()
    {
        var instanceIds = configContext.GetAllInstances().Select(i => i.Id).ToList();
        foreach (var id in instanceIds)
            instanceManager.Register(id);
    }
}