using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine;

public interface IPublish<in T> where T : class
{
    Task PublishAsync(T message, CancellationToken cancellationToken = default);
}