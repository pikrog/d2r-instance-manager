using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Engine.Agents;

using ProcessResult = Result<ProcessManager, ProcessError>;

public class StartProcessAgent(ProcessStartInfo startInfo, ILogger<StartProcessAgent> logger) : AgentBase<ProcessResult>
{
    protected override async Task<ProcessResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        var result = await ProcessManager.StartAsync(startInfo);

        if (result.IsSuccess)
        {
            logger.LogDebug("Process {ProcessId} started", result.Value.Id);
        }
        else
        {
            logger.LogError(
                "Process start failed: {Error} ({NativeErrorCode})",
                result.Error.FailureReason, result.Error.NativeErrorCode);
        }

        return result;
    }

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new ProcessStartFailed(exception);

    protected override Event MapAgentResultToEvent(ProcessResult result) =>
        result.IsSuccess ? new ProcessStarted(result.Value) : new ProcessStartFailed(result.Error);
}