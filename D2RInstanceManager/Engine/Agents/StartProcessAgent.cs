using System;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Common;
using D2RInstanceManager.Engine.Factories;
using D2RInstanceManager.Engine.Models.Contexts;
using D2RInstanceManager.Engine.Models.Events;
using D2RInstanceManager.Engine.Platform.Process;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Engine.Agents;

using ProcessResult = Result<ProcessManager, ProcessError>;

public class StartProcessAgent(ProcessStartContext startContext, ProcessStartRequestFactory requestFactory, ILogger<StartProcessAgent> logger) : AgentBase<ProcessResult>
{
    protected override async Task<ProcessResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        var request = requestFactory.Create(startContext);
        
        var redactedRequest = requestFactory.CreateRedacted(startContext);
        logger.LogDebug(
            "Starting process {GameExecutablePath}\r\nwith command-line arguments: {CommandLineArguments}",
            redactedRequest.FileName,
            redactedRequest.Arguments);
        
        var result = await ProcessManager.StartAsync(request);

        if (result.IsSuccess)
        {
            logger.LogDebug("Process {ProcessId} started", result.Value.Id);
        }
        else
        {
            logger.LogError("Process start failed. {ErrorMessage}", result.Error.Message);
        }

        return result;
    }

    protected override void OnGenericException(Exception exception) =>
        logger.LogError(exception, "Process start failed");

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new ProcessStartFailed(exception);

    protected override Event MapAgentResultToEvent(ProcessResult result) =>
        result.IsSuccess ? new ProcessStarted(result.Value) : new ProcessStartFailed(result.Error);
}