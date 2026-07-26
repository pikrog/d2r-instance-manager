using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Helpers.ProcessStop;
using AvaloniaApplication1.Engine.Helpers.ProcessStop.Error;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Agents;

using StopResult = Result<ProcessStopMode, ProcessStopError>;

public class StopProcessAgent(ProcessManager processManager, RetryingProcessStopper stopper) : AgentBase<StopResult>
{
    protected override Task<StopResult> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        stopper.StopAsync(processManager, cancellationToken);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new ProcessStopFailed(exception);

    protected override Event MapAgentResultToEvent(StopResult result) => 
        result.IsSuccess ? new ProcessStopped(result.Value) : new ProcessStopFailed(result.Error);
}