using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Common;
using D2RInstanceManager.Engine.Helpers.MultiboxUnlock;
using D2RInstanceManager.Engine.Models.Events;
using D2RInstanceManager.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Engine.Agents;

using MultiboxUnlockResult = Result<ProcessIdentity, RetryingMultiboxUnlockError>;

public class UnlockMultiboxAgent(RetryingMultiboxUnlocker unlocker, uint expectedProcessId, ILogger<UnlockMultiboxAgent> logger) 
    : AgentBase<MultiboxUnlockResult>
{
    protected override async Task<MultiboxUnlockResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        var result = await unlocker.UnlockAsync(cancellationToken);
        
        if (!result.IsSuccess)
        {
            logger.LogError("Multibox unlock failed. {Error}", result.Error);
        }
        else if (result.Value.Id != expectedProcessId)
        {
            logger.LogWarning(
                "Multibox unlocked for process {ProcessId} (expected {ExpectedProcessId})",
                result.Value.Id, expectedProcessId);
        }
        else
        {
            logger.LogDebug("Multibox unlocked for process {ProcessId}", result.Value.Id);
        }
        
        return result;
    }
    
    protected override void OnGenericException(Exception exception) => 
        logger.LogError(exception, "Multibox unlock failed");

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new MultiboxUnlockFailed(exception);

    protected override Event MapAgentResultToEvent(MultiboxUnlockResult result) => 
        result.IsSuccess ? new MultiboxUnlocked() : new MultiboxUnlockFailed(result.Error);
}