using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Config;
using D2RInstanceManager.Config.Stores;

namespace D2RInstanceManager.Design.Config;

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