using System.Threading.Tasks;
using AvaloniaApplication1.Common;

namespace AvaloniaApplication1.Page;

public abstract class PageViewModel : ViewModelBase
{
    public virtual Task OnEnterAsync() => Task.CompletedTask;

    public virtual Task OnLeaveAsync() => Task.CompletedTask;
    
    public virtual Task<bool> CanLeaveAsync() => Task.FromResult(true);
}