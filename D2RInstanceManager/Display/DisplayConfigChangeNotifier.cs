using System;
using D2RInstanceManager.Initialization;
using D2RInstanceManager.Toast;
using D2RInstanceManager.Toast.Descriptors;

namespace D2RInstanceManager.Display;

public class DisplayConfigChangeNotifier(IDisplayConfigMonitor monitor, ToastService toastService) : IRuntimeInitializer
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