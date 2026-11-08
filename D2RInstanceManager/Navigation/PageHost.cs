using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Navigation;

public partial class PageHost : ObservableObject, IPageHost, IPageController
{
    [ObservableProperty]
    public partial PageViewModel? CurrentPage { get; set; }
}