using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Common;

namespace AvaloniaApplication1.Engine.Helpers;

public class RetryingMultiboxUnlocker(RetryPolicy retryPolicy)
{
    public enum UnlockResult
    {
        Success,
        Timeout,
        CloseSourceFailed,
    }

    public async Task<UnlockResult> UnlockAsync(CancellationToken cancellationToken = default)
    {
        var retries = 0;
        while (retries < retryPolicy.MaxRetries)
        {
            ++retries;
            var result = await MultiboxUnlocker.UnlockAsync(cancellationToken);
            switch (result)
            {
                case MultiboxUnlocker.UnlockResult.Success:
                    return UnlockResult.Success;
                case MultiboxUnlocker.UnlockResult.EventNotFound:
                    await Task.Delay(retryPolicy.Delay, cancellationToken);
                    break;
                case MultiboxUnlocker.UnlockResult.CloseSourceFailed:
                    return UnlockResult.CloseSourceFailed;
                default:
                    throw new InvalidOperationException($"Unexpected multibox unlock result: {result}");
            }
        }
        return UnlockResult.Timeout;
    }
}