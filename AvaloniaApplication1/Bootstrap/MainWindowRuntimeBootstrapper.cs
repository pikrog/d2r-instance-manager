using Avalonia.Controls;
using AvaloniaApplication1.HotKey.Platform;
using AvaloniaApplication1.Instance;

namespace AvaloniaApplication1.Bootstrap;

public class MainWindowRuntimeBootstrapper(
    IHotKeyWindowInitializer hotKeyWindowInitializer,
    InstanceHotKeyBindingCoordinator instanceHotKeyBindingCoordinator)
{
    public void Bootstrap(Window mainWindow)
    {
        hotKeyWindowInitializer.Initialize(mainWindow);
        instanceHotKeyBindingCoordinator.Initialize();
    }
}
