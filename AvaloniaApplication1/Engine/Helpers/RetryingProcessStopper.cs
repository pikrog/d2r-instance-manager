using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Exceptions.Platform;
using AvaloniaApplication1.Engine.Lang;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Errors;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Models.Platform.Results;
using AvaloniaApplication1.Engine.Models.Results;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

using StopResult = Result<ProcessStopMode, ProcessStopError>;

public class RetryingProcessStopper(RetryPolicy closeRetryPolicy, TimeSpan killTimeout)
{
    public async Task<StopResult> StopAsync(Process process, CancellationToken token = default)
    {
        var retries = 0;
        while (retries < closeRetryPolicy.MaxRetries)
        {
            ++retries;
            
            var hasExitedCheck = process.CheckIfExited();
            if (!hasExitedCheck.IsSuccess)
                return StopResult.Failure(ProcessStopError.ProcessError(hasExitedCheck.Error));
            var hasExited = hasExitedCheck.Value;
            if (hasExited)
                return StopResult.Success(ProcessStopMode.Unknown);

            var closeRequestState = process.CloseMainWindow();

            using var closeTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            closeTimeoutCts.CancelAfter(closeRetryPolicy.Delay);

            try
            {
                await process.WaitForExitAsync(closeTimeoutCts.Token);
                return closeRequestState == RequestState.Accepted
                    ? StopResult.Success(ProcessStopMode.Graceful)
                    : StopResult.Success(ProcessStopMode.Unknown);
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested) { }
        }


        var killResult = process.Kill();
        if (!killResult.IsSuccess)
            return StopResult.Failure(ProcessStopError.ProcessError(killResult.Error));
        var killRequestState = killResult.Value;
        
        using var killTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        killTimeoutCts.CancelAfter(killTimeout);

        try
        {
            await process.WaitForExitAsync(killTimeoutCts.Token);
            return killRequestState == RequestState.Accepted
                ? StopResult.Success(ProcessStopMode.Forceful)
                : StopResult.Success(ProcessStopMode.Unknown);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return StopResult.Failure(ProcessStopError.Timeout());
        }
    }
}