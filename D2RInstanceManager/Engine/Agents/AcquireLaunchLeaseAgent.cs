using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Coordination;
using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Agents;

public class AcquireLaunchLeaseAgent(LaunchCoordinator coordinator) : AgentBase<LaunchLease>
{
    protected override async Task<LaunchLease> RunAgentTaskAsync(CancellationToken cancellationToken) => 
        await coordinator.AcquireAsync(cancellationToken);

    protected override Event CreateCanceledEvent() => new LaunchLeaseCanceled();

    protected override Event MapAgentResultToEvent(LaunchLease result) => new LaunchLeaseGranted(result);
}