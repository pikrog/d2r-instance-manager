using AvaloniaApplication1.Common;

namespace AvaloniaApplication1.Page;

public abstract class PageViewModel : ViewModelBase
{
    public abstract void OnEnter();

    public abstract bool OnLeave();
}