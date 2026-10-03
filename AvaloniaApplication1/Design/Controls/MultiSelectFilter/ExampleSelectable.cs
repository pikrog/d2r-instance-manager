using AvaloniaApplication1.Selectable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Design.Controls.MultiSelectFilter;

public partial class ExampleSelectable(string? label) : SelectableObservable
{
    [ObservableProperty] 
    public partial string? Label { get; set; } = label;
}