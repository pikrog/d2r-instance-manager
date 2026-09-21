using System.Linq;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Log;

namespace AvaloniaApplication1.Bootstrap;

public class InstanceRuntimeBootstrapper(ConfigContext configContext, InstanceManager instanceManager, InstanceNameRegistry instanceNameRegistry)
{
    public void Bootstrap()
    {
        var instances = configContext.GetAllInstances().ToList();
        foreach (var instance in instances)
        {
            instanceNameRegistry.Set(instance.Id, instance.Name);
            instanceManager.Register(instance.Id);
        }
    }
}
