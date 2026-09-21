using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine.Agents;

using MultiboxUnlockResult = Result<ProcessIdentity, RetryingMultiboxUnlockError>;

public class UnlockMultiboxAgent(RetryingMultiboxUnlocker unlocker, uint expectedProcessId, ILogger<UnlockMultiboxAgent> logger) 
    : AgentBase<MultiboxUnlockResult>
{
    protected override async Task<MultiboxUnlockResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        var result = await unlocker.UnlockAsync(cancellationToken);
        
        if (!result.IsSuccess)
        {
            logger.LogError("Multibox unlock failed: {Error}", result.Error);
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

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new MultiboxUnlockFailed(exception);

    protected override Event MapAgentResultToEvent(MultiboxUnlockResult result) => 
        result.IsSuccess ? new MultiboxUnlocked() : new MultiboxUnlockFailed(result.Error);
}