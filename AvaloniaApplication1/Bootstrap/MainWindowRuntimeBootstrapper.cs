using Avalonia.Controls;
using AvaloniaApplication1.HotKey;
using AvaloniaApplication1.Instance;

namespace AvaloniaApplication1.Bootstrap;

public class MainWindowRuntimeBootstrapper(
    IHotKeyWindowInitializer hotKeyWindowInitializer,
    InstanceHotKeyManager instanceHotKeyManager)
{
    public void Bootstrap(Window mainWindow)
    {
        hotKeyWindowInitializer.Initialize(mainWindow);
        instanceHotKeyManager.Initialize();
    }
}
