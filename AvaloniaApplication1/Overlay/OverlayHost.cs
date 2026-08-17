using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Overlay;

public partial class OverlayHost : ObservableObject, IOverlayHost, IOverlayController
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(IsOverlayVisible))]
    public partial IOverlayContent? OverlayContent { get; set; }
    
    public bool IsOverlayVisible => OverlayContent is not null;
}