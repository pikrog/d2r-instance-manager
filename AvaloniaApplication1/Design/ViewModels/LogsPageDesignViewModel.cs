using D2RInstanceManager.Design.Services;
using D2RInstanceManager.Log;
using Microsoft.Extensions.Logging.Abstractions;

namespace D2RInstanceManager.Design.ViewModels;

public class LogsPageDesignViewModel : LogsPageViewModel
{
    public LogsPageDesignViewModel() : base(
        DesignServices.LogStore,
        new LogSettings
        {
            MinimumLevel = LogLevel.Verbose
        },
        DesignServices.LogService,
        DesignServices.InstanceService,
        DesignServices.LogNotificationCounter,
        NullLogger<LogsPageDesignViewModel>.Instance)
    {
        Level = LogLevel.Verbose;
        
        base.OnEnterAsync().GetAwaiter().GetResult();
    }
}