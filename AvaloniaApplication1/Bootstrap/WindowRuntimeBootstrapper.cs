using System.Collections.Generic;
using Avalonia.Controls;
using D2RInstanceManager.Initialization;

namespace D2RInstanceManager.Bootstrap;

public class WindowRuntimeBootstrapper(IEnumerable<IWindowRuntimeInitializer> initializers)
{
    public void Bootstrap(Window window)
    {
        foreach (var initializer in initializers)
            initializer.Initialize(window);
    }
}