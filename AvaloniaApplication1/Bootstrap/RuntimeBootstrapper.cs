using System.Collections.Generic;
using D2RInstanceManager.Initialization;

namespace D2RInstanceManager.Bootstrap;

public class RuntimeBootstrapper(IEnumerable<IRuntimeInitializer> initializers)
{
    public void Bootstrap()
    {
        foreach (var initializer in initializers)
            initializer.Initialize();
    }
}
