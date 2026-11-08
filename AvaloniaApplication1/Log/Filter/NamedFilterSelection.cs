using CommunityToolkit.Mvvm.ComponentModel;

namespace D2RInstanceManager.Log.Filter;

public sealed partial class NamedFilterSelection(string name) : FilterSelection
{
    [ObservableProperty]
    public partial string Name { get; set; } = name;
}