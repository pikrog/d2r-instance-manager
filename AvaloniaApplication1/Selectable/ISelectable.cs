using System.ComponentModel;

namespace AvaloniaApplication1.Selectable;

public interface ISelectable : INotifyPropertyChanged
{
    bool IsSelected { get; set; }
}