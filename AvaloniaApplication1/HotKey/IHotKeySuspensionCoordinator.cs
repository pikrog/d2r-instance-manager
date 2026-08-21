using System;

namespace AvaloniaApplication1.HotKey;

public interface IHotKeySuspensionCoordinator
{
    IDisposable Suspend();
}