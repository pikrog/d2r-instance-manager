using System;
using System.IO;
using System.Threading.Tasks;
using AvaloniaApplication1.Snapshots;

// todo: move to Services
namespace AvaloniaApplication1.Config;

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