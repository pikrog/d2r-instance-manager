using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

public class AcquireLaunchLeaseAgent(LaunchCoordinator coordinator) : GameInstanceEngineAgentBase<CriticalSectionLease>
{
    protected override async Task<CriticalSectionLease> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        await coordinator.AcquireAsync(cancellationToken);

    protected override Event CreateCanceledEvent() => new LaunchLeaseCanceled();

    protected override Event MapAgentResultToEvent(CriticalSectionLease result) => new LaunchLeaseGranted(result);
}