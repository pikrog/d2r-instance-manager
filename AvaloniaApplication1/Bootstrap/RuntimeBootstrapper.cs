using System.Collections.Generic;
using AvaloniaApplication1.Initialization;

namespace AvaloniaApplication1.Bootstrap;

public class RuntimeBootstrapper(IEnumerable<IRuntimeInitializer> initializers)
{
    public void Bootstrap()
    {
        foreach (var initializer in initializers)
            initializer.Initialize();
    }
}
