using System.Linq;
using D2RInstanceManager.Config;
using D2RInstanceManager.Engine;
using D2RInstanceManager.Initialization;
using D2RInstanceManager.Log.Instance;

namespace D2RInstanceManager.Bootstrap;

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
