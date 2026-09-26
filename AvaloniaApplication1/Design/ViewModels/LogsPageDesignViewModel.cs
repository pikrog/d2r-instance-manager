using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Log;

namespace AvaloniaApplication1.Design.ViewModels;

public class LogsPageDesignViewModel : LogsPageViewModel
{
    public LogsPageDesignViewModel() : base(new FakeLogStore(), new LogSettings{ MinimumLevel = LogLevel.Verbose }, DesignServices.InstanceService)
    {
        Level = LogLevel.Verbose;
        
        base.OnEnterAsync().GetAwaiter().GetResult();
    }
}