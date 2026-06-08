using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Events.ProcessStop;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Agents;

public class StopProcessAgent(Process process, RetryingProcessStopper stopper) : GameInstanceEngineAgentBase<RetryingProcessStopper.StopResult>
{
    protected override Task<RetryingProcessStopper.StopResult> RunAgentTaskAsync(CancellationToken cancellationToken) => stopper.StopAsync(process, cancellationToken);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => new ProcessStopUnknownFailure(exception);

    protected override Event MapAgentResultToEvent(RetryingProcessStopper.StopResult result) =>
        result switch
        {
            RetryingProcessStopper.StopResult.Closed => new ProcessStopped(ProcessCloseMode.Graceful),
            RetryingProcessStopper.StopResult.Terminated => new ProcessStopped(ProcessCloseMode.Forceful),
            RetryingProcessStopper.StopResult.Timeout => new ProcessStopKnownFailure(ProcessStopFailureReason.Timeout),
            RetryingProcessStopper.StopResult.AlreadyExited => new ProcessStopped(ProcessCloseMode.Unknown),
            _ => throw new InvalidOperationException($"Unexpected process stop result: {result}")
        };
}