using System;
using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Design.Services;

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