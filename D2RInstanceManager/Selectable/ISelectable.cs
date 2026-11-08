using System.ComponentModel;

namespace D2RInstanceManager.Selectable;

public interface ISelectable : INotifyPropertyChanged
{
    bool IsSelected { get; set; }
}