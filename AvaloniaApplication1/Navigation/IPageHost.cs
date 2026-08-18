using System.ComponentModel;

namespace AvaloniaApplication1.Navigation;

public interface IPageHost : INotifyPropertyChanged
{
    PageViewModel? CurrentPage { get; }
}