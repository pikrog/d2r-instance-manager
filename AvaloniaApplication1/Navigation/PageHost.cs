using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Navigation;

public partial class PageHost : ObservableObject, IPageHost, IPageController
{
    [ObservableProperty]
    public partial PageViewModel? CurrentPage { get; set; }
}