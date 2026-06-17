using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Lang;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Agents;

using ExitCodeResult = Result<uint?, ProcessError>;

public class MonitorProcessExitAgent(Process process) : GameInstanceEngineAgentBase<ExitCodeResult>
{
    protected override async Task<ExitCodeResult> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        await process.WaitForExitAsync(cancellationToken);
        return process.ExitCode;
    }

    protected override Event MapAgentResultToEvent(ExitCodeResult result) =>
        result.IsSuccess ? new ProcessExited(result.Value) : new ProcessExited(Error: result.Error);
}