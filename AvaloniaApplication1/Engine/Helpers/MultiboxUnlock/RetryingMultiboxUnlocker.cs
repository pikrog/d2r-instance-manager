using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Common;
using D2RInstanceManager.Engine.Common;
using D2RInstanceManager.Engine.Platform.Process;

namespace D2RInstanceManager.Engine.Helpers.MultiboxUnlock;

using MultiboxUnlockResult = Result<ProcessIdentity, RetryingMultiboxUnlockError>;

public class RetryingMultiboxUnlocker(RetryPolicy retryPolicy)
{
    public async Task<MultiboxUnlockResult> UnlockAsync(CancellationToken cancellationToken = default)
    {
        var retries = 0;
        while (retries < retryPolicy.MaxRetries)
        {
            ++retries;
            var result = await MultiboxUnlocker.UnlockAsync(cancellationToken);
            if (result.IsSuccess)
                return MultiboxUnlockResult.Success(result.Value);
            switch (result.Error)
            {
                case MultiboxUnlockError.EventNotFound:
                    await Task.Delay(retryPolicy.Delay, cancellationToken);
                    break;
                case MultiboxUnlockError.CloseSourceFailed:
                    return MultiboxUnlockResult.Failure(RetryingMultiboxUnlockError.CloseSourceFailed);
                default:
                    throw new InvalidOperationException($"Unexpected multibox unlock result: {result}");
            }
        }
        return MultiboxUnlockResult.Failure(RetryingMultiboxUnlockError.Timeout);
    }
}