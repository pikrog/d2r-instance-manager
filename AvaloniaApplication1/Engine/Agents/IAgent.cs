using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Agents;

public interface IAgent
{
    Task<Event?> RunAsync(CancellationToken cancellationToken);
}