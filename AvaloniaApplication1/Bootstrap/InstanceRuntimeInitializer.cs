using System.Linq;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.Engine;
using AvaloniaApplication1.Initialization;
using AvaloniaApplication1.Log.Instance;

namespace AvaloniaApplication1.Bootstrap;

public class InstanceRuntimeInitializer(
    ConfigContext configContext,
    InstanceManager instanceManager,
    InstanceNameRegistry instanceNameRegistry) : IRuntimeInitializer
{
    public void Initialize()
    {
        var instances = configContext.GetAllInstances().ToList();
        foreach (var instance in instances)
        {
            instanceNameRegistry.Set(instance.Id, instance.Name);
            instanceManager.Register(instance.Id);
        }
    }
}
