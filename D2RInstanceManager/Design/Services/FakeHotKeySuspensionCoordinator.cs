using System;
using D2RInstanceManager.HotKey.Platform;

namespace D2RInstanceManager.Design.Services;

public class FakeHotKeySuspensionCoordinator : IHotKeySuspensionCoordinator
{
    private class DummyDisposable : IDisposable
    {
        public void Dispose()
        {
        }
    }
    
    public IDisposable Suspend() => new DummyDisposable();
}