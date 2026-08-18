namespace AvaloniaApplication1.Navigation;

public interface IPageController
{
    PageViewModel? CurrentPage { get; set; }
}