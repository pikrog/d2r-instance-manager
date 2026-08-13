using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.MultiboxUnlock;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

using MultiboxUnlockResult = Result<Unit, RetryingMultiboxUnlockError>;

public class UnlockMultiboxAgent(RetryingMultiboxUnlocker unlocker) : AgentBase<MultiboxUnlockResult>
{
    protected override async Task<MultiboxUnlockResult> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        await unlocker.UnlockAsync(cancellationToken);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new MultiboxUnlockFailed(exception);

    protected override Event MapAgentResultToEvent(MultiboxUnlockResult result) => 
        result.IsSuccess ? new MultiboxUnlocked() : new MultiboxUnlockFailed(result.Error);
}