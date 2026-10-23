using System;
using AvaloniaApplication1.Display;

namespace AvaloniaApplication1.Design.Services;

public class FakeDisplayConfigMonitor : IDisplayConfigMonitor
{
    public event EventHandler? DisplayConfigChanged;
    
    public void TriggerDisplayConfigChanged() => DisplayConfigChanged?.Invoke(this, EventArgs.Empty);
}