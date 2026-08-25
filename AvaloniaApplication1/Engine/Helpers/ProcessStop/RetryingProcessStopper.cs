using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;
using AvaloniaApplication1.Engine.Platform;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Helpers.ProcessStop;

using StopResult = Result<ProcessStopMode, ProcessStopError>;

public class RetryingProcessStopper(ProcessStopPolicies policies)
{
    public const uint DefaultForcefulExitCode = 0xe0000001;
    
    public uint ForcefulExitCode => policies.ForcefulExitCode;
    
    public async Task<StopResult> StopAsync(ProcessManager processManager, CancellationToken token = default)
    {
        var retries = 0;
        while (retries < policies.GracefulProcessStopRetryPolicy.MaxRetries)
        {
            ++retries;
            
            var hasExitedCheck = processManager.CheckIfExited();
            if (!hasExitedCheck.IsSuccess)
            {
                var error = new ProcessStopOperationError(hasExitedCheck.Error);
                return StopResult.Failure(error);
            }

            var hasExited = hasExitedCheck.Value;
            if (hasExited)
                return StopResult.Success(ProcessStopMode.Unknown);

            var closeRequestState = processManager.CloseMainWindow();

            using var closeTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
            closeTimeoutCts.CancelAfter(policies.GracefulProcessStopRetryPolicy.Delay);

            try
            {
                await processManager.WaitForExitAsync(closeTimeoutCts.Token);
                return closeRequestState == RequestState.Accepted
                    ? StopResult.Success(ProcessStopMode.Graceful)
                    : StopResult.Success(ProcessStopMode.Unknown);
            }
            catch (OperationCanceledException) when (!token.IsCancellationRequested) { }
        }


        var killResult = processManager.Kill(policies.ForcefulExitCode);
        if (!killResult.IsSuccess)
        {
            var error = new ProcessStopOperationError(killResult.Error);
            return StopResult.Failure(error);
        }

        var killRequestState = killResult.Value;
        
        using var killTimeoutCts = CancellationTokenSource.CreateLinkedTokenSource(token);
        killTimeoutCts.CancelAfter(policies.ForcefulProcessStopTimeout);

        try
        {
            await processManager.WaitForExitAsync(killTimeoutCts.Token);
            return killRequestState == RequestState.Accepted
                ? StopResult.Success(ProcessStopMode.Forceful)
                : StopResult.Success(ProcessStopMode.Unknown);
        }
        catch (OperationCanceledException) when (!token.IsCancellationRequested)
        {
            return StopResult.Failure(new ProcessStopTimeout());
        }
    }
}