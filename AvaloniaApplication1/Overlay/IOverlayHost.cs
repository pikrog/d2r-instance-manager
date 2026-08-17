using System.ComponentModel;

namespace AvaloniaApplication1.Overlay;

public interface IOverlayHost : INotifyPropertyChanged
{
    IOverlayContent? OverlayContent { get; }
    bool IsOverlayVisible { get; }
}