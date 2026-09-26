using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Log.Instance;

public sealed partial class NamedInstanceFilterSelection(string name) : InstanceFilterSelection
{
    [ObservableProperty]
    public partial string Name { get; set; } = name;
}