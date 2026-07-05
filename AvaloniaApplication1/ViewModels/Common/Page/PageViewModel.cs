namespace AvaloniaApplication1.ViewModels.Common.Page;

public abstract class PageViewModel : ViewModelBase
{
    public abstract void OnEnter();

    public abstract bool OnLeave();
}