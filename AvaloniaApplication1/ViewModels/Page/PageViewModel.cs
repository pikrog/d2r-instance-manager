namespace AvaloniaApplication1.ViewModels.Page;

public abstract class PageViewModel : ViewModelBase
{
    public abstract void OnEnter();

    public abstract bool OnLeave();
}