using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Common;
using D2RInstanceManager.Engine.Helpers.ProcessStop;
using D2RInstanceManager.Engine.Helpers.ProcessStop.Error;
using D2RInstanceManager.Engine.Models.Events;
using D2RInstanceManager.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Engine.Agents;

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