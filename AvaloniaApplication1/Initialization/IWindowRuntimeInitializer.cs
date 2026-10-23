using Avalonia.Controls;

namespace AvaloniaApplication1.Initialization;

public interface IWindowRuntimeInitializer
{
    void Initialize(Window window);
}