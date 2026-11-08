namespace D2RInstanceManager.Navigation;

public interface IPageController
{
    PageViewModel? CurrentPage { get; set; }
}