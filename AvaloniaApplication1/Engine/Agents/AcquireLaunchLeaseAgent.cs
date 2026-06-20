using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Coordination;
using AvaloniaApplication1.Engine.Helpers;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Agents;

public class AcquireLaunchLeaseAgent(LaunchCoordinator coordinator) : AgentBase<LaunchLease>
{
    protected override async Task<LaunchLease> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        await coordinator.AcquireAsync(cancellationToken);

    protected override Event CreateCanceledEvent() => new LaunchLeaseCanceled();

    protected override Event MapAgentResultToEvent(LaunchLease result) => new LaunchLeaseGranted(result);
}