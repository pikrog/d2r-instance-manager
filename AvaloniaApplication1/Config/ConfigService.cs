using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config.Stores;

namespace AvaloniaApplication1.Config;

public class ConfigService(ConfigContext configContext, IConfigStore configStore)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    public IConfigReader Config => configContext;
    
    public async Task ChangeAsync(Action<ConfigContext> change)
    {
        await _semaphore.WaitAsync();
        try
        {
            change(configContext);
            var config = configContext.GetAppConfigCopy();
            await configStore.SaveAsync(config);
        }
        finally
        {
            _semaphore.Release();
        }
    }
}