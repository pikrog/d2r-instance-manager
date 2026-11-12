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
        else switch (result.Error)
        {
            case ProcessStopOperationError { Error: var processError }:
                logger.LogError(
                    "Failed to stop process {ProcessId}. {ErrorMessage})", 
                    processManager.Id, processError.Message);
                break;
            case ProcessStopTimeout:
                logger.LogError("Failed to stop process {ProcessId} (timeout)", processManager.Id);
                break;
            default:
                throw new InvalidOperationException("Unexpected process stop error");
        }
        
        return result;
            
    }
    
    protected override void OnGenericException(Exception exception) =>
        logger.LogError(exception, "Failed to stop process {ProcessId}", processManager.Id);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new ProcessStopFailed(exception);

    protected override Event MapAgentResultToEvent(StopResult result) => 
        result.IsSuccess ? new ProcessStopped(result.Value) : new ProcessStopFailed(result.Error);
}