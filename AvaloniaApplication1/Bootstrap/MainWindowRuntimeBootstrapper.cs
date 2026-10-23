using Avalonia.Controls;
using AvaloniaApplication1.Display;
using AvaloniaApplication1.HotKey.Platform;
using AvaloniaApplication1.Instance;

namespace AvaloniaApplication1.Bootstrap;

public class MainWindowRuntimeBootstrapper(
    IHotKeyWindowInitializer hotKeyWindowInitializer,
    InstanceHotKeyBindingCoordinator instanceHotKeyBindingCoordinator,
    IDisplayConfigMonitorWindowInitializer displayConfigMonitorWindowInitializer,
    DisplayConfigChangeNotifier displayConfigChangeNotifier)
{
    public void Bootstrap(Window mainWindow)
    {
        hotKeyWindowInitializer.Initialize(mainWindow);
        instanceHotKeyBindingCoordinator.Initialize();
        
        displayConfigMonitorWindowInitializer.Initialize(mainWindow);
        displayConfigChangeNotifier.Initialize();
    }
}
