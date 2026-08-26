using System;

namespace AvaloniaApplication1.HotKey.Platform;

public interface IHotKeySuspensionCoordinator
{
    IDisposable Suspend();
}