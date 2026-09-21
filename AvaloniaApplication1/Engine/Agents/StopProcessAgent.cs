using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine.Agents;

using StopResult = Result<ProcessStopMode, ProcessStopError>;

public class StopProcessAgent(ProcessManager processManager, RetryingProcessStopper stopper, ILogger<StopProcessAgent> logger) 
    : AgentBase<StopResult>
{
    protected override async Task<StopResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        var result = await stopper.StopAsync(processManager, cancellationToken);

        if (result.IsSuccess)
        {
            logger.LogTrace("Process {ProcessId} stopped (mode: {ProcessStopMode})", processManager.Id, result.Value);
        }
        else if (result.Error is ProcessStopOperationError { Error: var processError })
        {
            logger.LogError(
                "Failed to stop process {ProcessId}: {Error} ({NativeErrorCode})",
                processManager.Id, processError.FailureReason, processError.NativeErrorCode);
        }
        else if (result.Error is ProcessStopTimeout)
        {
            logger.LogError("Failed to stop process {ProcessId} (timeout)", processManager.Id);
        }
        else
            throw new InvalidOperationException("Unexpected process stop error");
        
        return result;
            
    }

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new ProcessStopFailed(exception);

    protected override Event MapAgentResultToEvent(StopResult result) => 
        result.IsSuccess ? new ProcessStopped(result.Value) : new ProcessStopFailed(result.Error);
}