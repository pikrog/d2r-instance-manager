using System.Collections.Generic;
using Avalonia.Controls;
using AvaloniaApplication1.Initialization;

namespace AvaloniaApplication1.Bootstrap;

public class WindowRuntimeBootstrapper(IEnumerable<IWindowRuntimeInitializer> initializers)
{
    public void Bootstrap(Window window)
    {
        foreach (var initializer in initializers)
            initializer.Initialize(window);
    }
}