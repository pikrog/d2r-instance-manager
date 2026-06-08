using System;
using System.Threading;

namespace AvaloniaApplication1.Engine;

public sealed class CriticalSectionLease(SemaphoreSlim semaphore) : IDisposable
{
    private int _disposed = 0;
        
    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
            return;
            
        semaphore.Release();
    }
}