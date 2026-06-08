using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Events;
using AvaloniaApplication1.Engine.Models.Events.ProcessExit;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Agents;

public class MonitorProcessExitAgent(Process process) : GameInstanceEngineAgentBase<uint>
{
    protected override async Task<uint> RunAgentTaskAsync(CancellationToken cancellationToken)
    {
        await process.WaitForExitAsync(cancellationToken);
        return process.ExitCode;
    }

    protected override Event MapAgentResultToEvent(uint result) => new ProcessExited(result);
}