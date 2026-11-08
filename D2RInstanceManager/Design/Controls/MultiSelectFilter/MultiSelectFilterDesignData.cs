using System.Collections.ObjectModel;
using D2RInstanceManager.Selectable;

namespace D2RInstanceManager.Design.Controls.MultiSelectFilter;

public static partial class MultiSelectFilterDesignData
{
    public static readonly ReadOnlyObservableCollection<ExampleSelectable> ExampleSelectables = 
        new(
            new ObservableCollection<ExampleSelectable>(
            [
                new ExampleSelectable("Example 1"), 
                new ExampleSelectable("Example 2"), 
                new ExampleSelectable("Example 3")
            ]));

    public static readonly SelectedItemsCollection<ExampleSelectable> SelectedItemsCollection = new(ExampleSelectables);
}