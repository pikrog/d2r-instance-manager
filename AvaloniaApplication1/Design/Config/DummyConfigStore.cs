using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Design.Config;

public class DummyConfigStore : IConfigStore
{
    public Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        return Task.CompletedTask;
    }

    public Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default)
    {
        return new Task<AppConfig>(() => new AppDesignConfig());
    }
}