using System;
using D2RInstanceManager.Display;

namespace D2RInstanceManager.Design.Services;

public class FakeDisplayConfigMonitor : IDisplayConfigMonitor
{
    public event EventHandler? DisplayConfigChanged;
    
    public void TriggerDisplayConfigChanged() => DisplayConfigChanged?.Invoke(this, EventArgs.Empty);
}