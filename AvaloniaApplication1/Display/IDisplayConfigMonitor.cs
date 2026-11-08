using System;

namespace D2RInstanceManager.Display;

public interface IDisplayConfigMonitor
{
    event EventHandler DisplayConfigChanged;
}