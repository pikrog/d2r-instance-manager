using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Lang;
using AvaloniaApplication1.Engine.Models.Errors;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Models.Results;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Agents;

using StopResult = Result<ProcessStopMode, ProcessStopError>;

public class StopProcessAgent(Process process, RetryingProcessStopper stopper) : GameInstanceEngineAgentBase<StopResult>
{
    protected override Task<StopResult> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        stopper.StopAsync(process, cancellationToken);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) => 
        new ProcessStopFailed(exception);

    protected override Event MapAgentResultToEvent(StopResult result) => 
        result.IsSuccess ? new ProcessStopped(result.Value) : new ProcessStopFailed(result.Error);
}