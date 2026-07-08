using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Common;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Platform.Process;

namespace AvaloniaApplication1.Engine.Agents;

using ExitCodeResult = Result<uint?, ProcessError>;

public class MonitorProcessExitAgent(ProcessManager processManager) : AgentBase<ExitCodeResult>
{
    protected override async Task<ExitCodeResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        await processManager.WaitForExitAsync(cancellationToken);
        return processManager.ExitCode;
    }

    protected override Event MapAgentResultToEvent(ExitCodeResult result) =>
        result.IsSuccess ? new ProcessExited(result.Value) : new ProcessExited(Error: result.Error);
}