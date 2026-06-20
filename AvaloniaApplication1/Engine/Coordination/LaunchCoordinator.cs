using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine.Coordination;

public class LaunchCoordinator
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public async Task<LaunchLease> AcquireAsync(CancellationToken cancellationToken = default)
    {
        await _semaphore.WaitAsync(cancellationToken);
        return new LaunchLease(_semaphore);
    }
}