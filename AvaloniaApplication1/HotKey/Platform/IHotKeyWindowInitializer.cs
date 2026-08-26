using Avalonia.Controls;

namespace AvaloniaApplication1.HotKey.Platform;

public interface IHotKeyWindowInitializer
{
    public void Initialize(Window window);
}