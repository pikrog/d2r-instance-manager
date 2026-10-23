using System;

namespace AvaloniaApplication1.Display;

public interface IDisplayConfigMonitor
{
    event EventHandler DisplayConfigChanged;
}