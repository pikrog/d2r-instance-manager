using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Agents;

using ExitCodeResult = Result<uint?, ProcessError>;

public class MonitorProcessExitAgent(ProcessManager processManager, uint forcefulExitCode) : AgentBase<ExitCodeResult>
{
    protected override async Task<ExitCodeResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        await processManager.WaitForExitAsync(cancellationToken);
        return processManager.ExitCode;
    }


    protected override Event MapAgentResultToEvent(ExitCodeResult result)
    {
        if (!result.IsSuccess)
            return new ProcessExited(ProcessExitResult.Unknown, result.Error);

        if (result.Value == forcefulExitCode)
            return new ProcessExited(ProcessExitResult.Terminated);

        return result.Value == 0 
            ? new ProcessExited(ProcessExitResult.Success) 
            : new ProcessExited(ProcessExitResult.Failure);
    }
}