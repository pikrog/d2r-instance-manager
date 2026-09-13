using AvaloniaApplication1.Design.Services;
using AvaloniaApplication1.Log;

namespace AvaloniaApplication1.Design.ViewModels;

public class LogsPageDesignViewModel : LogsPageViewModel
{
    public LogsPageDesignViewModel() : base(new FakeLogStore())
    {
        base.OnEnterAsync().GetAwaiter().GetResult();
    }
}