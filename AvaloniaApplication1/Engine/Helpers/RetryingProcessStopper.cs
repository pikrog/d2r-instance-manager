using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Exceptions.Platform;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

public class RetryingProcessStopper(RetryPolicy closeRetryPolicy, TimeSpan killTimeout)
{
    public enum StopResult
    {
        Closed,
        Terminated,
        Timeout,
        AlreadyExited,
    }

    public async Task<StopResult> StopAsync(Process process, CancellationToken token = default)
    {
        var retries = 0;
        while (retries < closeRetryPolicy.MaxRetries)
        {
            ++retries;

            try
            {
                process.CloseMainWindow();
            }
            catch (ProcessAlreadyExitedException)
            {
                return StopResult.AlreadyExited;
            }

            using var closeTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            closeTimeoutCts.CancelAfter(closeRetryPolicy.Delay);

            try
            {
                await process.WaitForExitAsync(closeTimeoutCts.Token);
                return StopResult.Closed;
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested) { }
        }

        try
        {
            process.Kill();
        }
        catch (ProcessAlreadyExitedException)
        {
            return StopResult.AlreadyExited;
        }
        
        using var killTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        killTimeoutCts.CancelAfter(killTimeout);

        try
        {
            await process.WaitForExitAsync(killTimeoutCts.Token);
            return StopResult.Terminated;
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return StopResult.Timeout;
        }
    }
}