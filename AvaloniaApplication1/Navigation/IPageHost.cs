using System.ComponentModel;

namespace D2RInstanceManager.Navigation;

public interface IPageHost : INotifyPropertyChanged
{
    PageViewModel? CurrentPage { get; }
}