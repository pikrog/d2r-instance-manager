using System.Collections;
using System.Collections.Generic;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Behaviors;

public class DataGridSelectedItemsBehavior : AvaloniaObject
{
    public static readonly AttachedProperty<IList?> SelectedItemsProperty =
        AvaloniaProperty.RegisterAttached<DataGridSelectedItemsBehavior, DataGrid, IList?>("SelectedItems");
    
    public static void SetSelectedItems(DataGrid target, IList? value) => target.SetValue(SelectedItemsProperty, value);
    public static IList? GetSelectedItems(DataGrid target) => target.GetValue(SelectedItemsProperty);
    
    static DataGridSelectedItemsBehavior()
    {
        SelectedItemsProperty.Changed.AddClassHandler<DataGrid>(OnSelectedItemsPropertyChanged);
    }

    private static void OnSelectedItemsPropertyChanged(DataGrid grid, AvaloniaPropertyChangedEventArgs e)
    {
        grid.SelectionChanged -= OnGridSelectionChanged;
        grid.SelectionChanged += OnGridSelectionChanged;
    }

    private static void OnGridSelectionChanged(object? sender, SelectionChangedEventArgs e)
    {
        if (sender is not DataGrid grid)
            return;

        var target = grid.GetValue(SelectedItemsProperty);
        if (target is null)
            return;

        target.Clear();
        foreach (var item in grid.SelectedItems)
            target.Add(item);
    }
}