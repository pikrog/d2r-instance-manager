using Avalonia.Controls;

namespace D2RInstanceManager.Initialization;

public interface IWindowRuntimeInitializer
{
    void Initialize(Window window);
}