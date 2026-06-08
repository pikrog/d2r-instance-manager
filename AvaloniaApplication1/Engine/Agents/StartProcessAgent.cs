using System;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Events.ProcessStart;
using AvaloniaApplication1.Engine.Models.Events.ProcessStop;
using ProcessStartInfo = AvaloniaApplication1.Engine.Models.Platform.Process.ProcessStartInfo;


namespace AvaloniaApplication1.Engine.Agents;

public class StartProcessAgent(ProcessStartInfo startInfo) : GameInstanceEngineAgentBase<ProcessStarter.StartResult>
{
    protected override Task<ProcessStarter.StartResult> RunAgentTaskAsync(CancellationToken cancellationToken) =>
        Task.FromResult(ProcessStarter.Start(startInfo));

    protected override ErrorEvent CreateErrorForGenericException(Exception exception) =>
        new ProcessStartUnknownFailure(exception);

    protected override Event MapAgentResultToEvent(ProcessStarter.StartResult result) =>
        result switch
        {
            ProcessStarter.StartResult.Success r => new ProcessStarted(r.Process),
            ProcessStarter.StartResult.Failure r => r.Reason switch
            {
                ProcessStarter.FailureReason.FileNotFound => new ProcessStartKnownFailure(ProcessStartFailureReason
                    .FileNotFound),
                ProcessStarter.FailureReason.AccessDenied => new ProcessStartKnownFailure(ProcessStartFailureReason
                    .AccessDenied),
                _ => throw new InvalidOperationException($"Unexpected failure reason: {r.Reason}")
            },
            _ => throw new InvalidOperationException($"Unexpected process start result type: {result.GetType().Name}")
        };
}