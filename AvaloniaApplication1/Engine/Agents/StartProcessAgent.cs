using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Agents;

using ProcessResult = Result<ProcessManager, ProcessError>;

public class StartProcessAgent(ProcessStartInfo startInfo) : AgentBase<ProcessResult>
{
    protected override async Task<ProcessResult> RunAgentTaskAsync(CancellationToken cancellationToken) =>
        await ProcessManager.StartAsync(startInfo);

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new ProcessStartFailed(exception);

    protected override Event MapAgentResultToEvent(ProcessResult result) =>
        result.IsSuccess ? new ProcessStarted(result.Value) : new ProcessStartFailed(result.Error);
}