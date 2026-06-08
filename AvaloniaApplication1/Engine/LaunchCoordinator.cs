using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine;

public class LaunchCoordinator
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<CriticalSectionLease> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        return new CriticalSectionLease(_semaphore);
    }
}