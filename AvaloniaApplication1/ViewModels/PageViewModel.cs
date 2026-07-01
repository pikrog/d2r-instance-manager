namespace AvaloniaApplication1.ViewModels;

public abstract class PageViewModel : ViewModelBase
{
    public abstract void OnEnter();

    public abstract bool OnLeave();
}