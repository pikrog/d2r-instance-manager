using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Lang;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Agents;

using ProcessResult = Result<Process, ProcessError>;

public class StartProcessAgent(ProcessStartInfo startInfo) : GameInstanceEngineAgentBase<ProcessResult>
{
    protected override Task<ProcessResult> RunAgentTaskAsync(CancellationToken cancellationToken) =>
        Task.FromResult(Process.Start(startInfo));

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new ProcessStartFailed(exception);

    protected override Event MapAgentResultToEvent(ProcessResult result) =>
        result.IsSuccess ? new ProcessStarted(result.Value) : new ProcessStartFailed(result.Error);
}