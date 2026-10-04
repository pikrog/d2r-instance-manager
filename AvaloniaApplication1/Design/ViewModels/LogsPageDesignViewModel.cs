using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Log;
using Microsoft.Extensions.Logging.Abstractions;

namespace AvaloniaApplication1.Design.ViewModels;

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
        new NullLogger<LogsPageViewModel>())
    {
        Level = LogLevel.Verbose;
        
        base.OnEnterAsync().GetAwaiter().GetResult();
    }
}