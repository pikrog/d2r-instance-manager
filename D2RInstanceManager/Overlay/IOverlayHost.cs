using System.ComponentModel;

namespace D2RInstanceManager.Overlay;

public interface IOverlayHost : INotifyPropertyChanged
{
    IOverlayContent? OverlayContent { get; }
    bool IsOverlayVisible { get; }
}