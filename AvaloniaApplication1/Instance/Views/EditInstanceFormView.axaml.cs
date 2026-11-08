using Avalonia.Controls;
using D2RInstanceManager.HotKey.Platform;

namespace D2RInstanceManager.Instance.Views;

public partial class EditInstanceFormView : Window
{
    public IHotKeySuspensionCoordinator HotKeySuspensionCoordinator { get; }
    
    public EditInstanceFormView(IHotKeySuspensionCoordinator coordinator)
    {
        HotKeySuspensionCoordinator = coordinator;
        
        InitializeComponent();
    }
}