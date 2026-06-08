using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Events.MultiboxUnlock;

namespace AvaloniaApplication1.Engine.Agents;

public class UnlockMultiboxAgent(RetryingMultiboxUnlocker unlocker) : GameInstanceEngineAgentBase<RetryingMultiboxUnlocker.UnlockResult>
{
    protected override async Task<RetryingMultiboxUnlocker.UnlockResult> RunAgentTaskAsync(CancellationToken cancellationToken) => await unlocker.UnlockAsync(cancellationToken);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => new MultiboxUnlockUnknownFailure(exception);

    protected override Event MapAgentResultToEvent(RetryingMultiboxUnlocker.UnlockResult result) =>
        result switch
        {
            RetryingMultiboxUnlocker.UnlockResult.Success => new MultiboxUnlocked(),
            RetryingMultiboxUnlocker.UnlockResult.Timeout => new MultiboxUnlockKnownFailure(MultiboxUnlockFailureReason.Timeout),
            RetryingMultiboxUnlocker.UnlockResult.CloseSourceFailed => new MultiboxUnlockKnownFailure(MultiboxUnlockFailureReason.CloseSourceFailed),
            _ => throw new InvalidOperationException($"Unexpected multibox unlock result: {result}")
        };
}