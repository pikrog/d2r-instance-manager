using Avalonia.Controls;
using AvaloniaApplication1.HotKey.Platform;

namespace AvaloniaApplication1.Instance.Views;

public partial class EditInstanceFormView : Window
{
    public IHotKeySuspensionCoordinator HotKeySuspensionCoordinator { get; }
    
    public EditInstanceFormView(IHotKeySuspensionCoordinator coordinator)
    {
        HotKeySuspensionCoordinator = coordinator;
        
        InitializeComponent();
    }
}