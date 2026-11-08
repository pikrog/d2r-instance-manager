using System;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config.Stores;

namespace AvaloniaApplication1.Config;

public class ConfigService(AppConfig appConfig, IConfigStore configStore)
{
    private readonly SemaphoreSlim _semaphore = new(1, 1);
    
    private ConfigContext _configContext = new(appConfig);

    public IConfigReader Config => _configContext;
    
    public async Task ChangeAsync(Action<ConfigContext> change)
    {
        await _semaphore.WaitAsync();
        try
        {
            var workingConfig = _configContext.GetAppConfigCopy();
            var workingContext = new ConfigContext(workingConfig);
            
            change(workingContext);
            
            await configStore.SaveAsync(workingConfig);
            
            _configContext = workingContext;
        }
        finally
        {
            _semaphore.Release();
        }
    }
}