using System.Threading.Tasks;
using D2RInstanceManager.Common;

namespace D2RInstanceManager.Navigation;

public abstract class PageViewModel : ViewModelBase
{
    public virtual Task OnEnterAsync() => Task.CompletedTask;

    public virtual Task OnLeaveAsync() => Task.CompletedTask;
    
    public virtual Task<bool> CanLeaveAsync() => Task.FromResult(true);
}