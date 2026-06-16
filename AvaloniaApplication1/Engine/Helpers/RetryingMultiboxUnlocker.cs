using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Errors;

namespace AvaloniaApplication1.Engine.Helpers;

using MultiboxUnlockResult = Result<Unit, RetryingMultiboxUnlockError>;

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
                return MultiboxUnlockResult.Success();
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