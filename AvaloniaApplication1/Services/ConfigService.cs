using System;
using System.Threading.Tasks;
using AvaloniaApplication1.Config;

namespace AvaloniaApplication1.Services;

public class ConfigService(ConfigContext configContext, IConfigStore configStore)
{
    public IConfigReader Config => configContext;
    
    public async Task ChangeAsync(Action<ConfigContext> change)
    {
        change(configContext);
        var config = configContext.GetAppConfigCopy();
        await configStore.SaveAsync(config);
    }
}