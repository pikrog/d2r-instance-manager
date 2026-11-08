using System;

namespace D2RInstanceManager.HotKey.Platform;

public interface IHotKeySuspensionCoordinator
{
    IDisposable Suspend();
}