using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Selectable;

public partial class SelectableObservable : ObservableObject, ISelectable
{
    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}