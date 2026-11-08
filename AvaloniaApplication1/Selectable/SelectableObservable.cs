using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Selectable;

public partial class SelectableObservable : ObservableObject, ISelectable
{
    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}