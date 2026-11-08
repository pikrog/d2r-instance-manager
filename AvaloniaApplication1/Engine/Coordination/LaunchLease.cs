using System;
using System.Threading;

namespace D2RInstanceManager.Engine.Coordination;

public sealed class LaunchLease(SemaphoreSlim semaphore) : IDisposable
{
    private int _disposed = 0;
        
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
            return;
            
        semaphore.Release();
    }
}