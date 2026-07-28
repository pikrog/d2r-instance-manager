using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Selectable;

public partial class SelectableViewModelBase : ViewModelBase
{
    [ObservableProperty]
    public partial bool IsSelected { get; set; }
}