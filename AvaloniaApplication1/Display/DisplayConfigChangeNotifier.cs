using System;
using AvaloniaApplication1.Toast;
using AvaloniaApplication1.Toast.Descriptors;

namespace AvaloniaApplication1.Display;

public class DisplayConfigChangeNotifier(IDisplayConfigMonitor monitor, ToastService toastService)
{
    public void Initialize()
    {
        monitor.DisplayConfigChanged += OnDisplayConfigChanged;
    }

    private void OnDisplayConfigChanged(object? sender, EventArgs e)
    {
        var toast = new DisplayConfigChanged();
        toastService.Show(toast);
    }
}