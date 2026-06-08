using System.Threading;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Config;

public interface IConfigStore
{
    public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default);
    public Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default);
}