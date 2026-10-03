using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using AvaloniaApplication1.Log.Filter;
using AvaloniaApplication1.Selectable;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Log;

public class NameFilter : ObservableObject
{
    private readonly HashSet<string> _names = [];
    
    private readonly NoNameFilterSelection _noNameSelection = new();

    private readonly ObservableCollection<FilterSelection> _selections;

    public ReadOnlyObservableCollection<FilterSelection> Selections { get; }

    public SelectedItemsCollection<FilterSelection> ActiveSelections { get; }
    
    public HashSet<string> ActiveNames => ActiveSelections.Items.OfType<NamedFilterSelection>()
        .Select(s => s.Name)
        .ToHashSet();
    
    public bool IsNoNameActive => _noNameSelection.IsSelected;

    public bool IsOnlyNoNameActive => 
        ActiveSelections.Count == 1 && _noNameSelection.IsSelected;
    
    public bool IsOnlySingleNameActive =>
        ActiveSelections.Count == 1 && !_noNameSelection.IsSelected;

    public string? FirstActiveName =>
        ActiveSelections.Items
            .OfType<NamedFilterSelection>()
            .FirstOrDefault()
            ?.Name;

    public bool AreOnlyMultipleNamesActive =>
        ActiveSelections.Count > 1 && !_noNameSelection.IsSelected;
    
    public bool AreSingleNameAndNoNameActive =>
        ActiveSelections.Count == 2 && _noNameSelection.IsSelected;
    
    public bool AreMultipleNamesAndNoNameActive =>
        ActiveSelections.Count > 2 && _noNameSelection.IsSelected;

    public int ActiveNamesCount =>
        ActiveSelections.Items.OfType<NamedFilterSelection>().Count();
    
    public bool IsMixedFilteringActive =>
        ActiveSelections.Count > 1 && _noNameSelection.IsSelected;

    public NameFilter(IEnumerable<string> names)
    {
        var selections = names.Select(FilterSelection (n) => new NamedFilterSelection(n))
            .Append(_noNameSelection);
        
        _selections = new ObservableCollection<FilterSelection>(selections);
        Selections = new ReadOnlyObservableCollection<FilterSelection>(_selections);
        ActiveSelections = new SelectedItemsCollection<FilterSelection>(Selections);
        ActiveSelections.CollectionChanged += OnActiveSelectionsChanged; 
    }
    
    public NameFilter() : this([]) {}

    private void OnActiveSelectionsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(IsNoNameActive));
        OnPropertyChanged(nameof(IsOnlyNoNameActive));
        OnPropertyChanged(nameof(IsOnlySingleNameActive));
        OnPropertyChanged(nameof(FirstActiveName));
        OnPropertyChanged(nameof(AreOnlyMultipleNamesActive));
        OnPropertyChanged(nameof(AreSingleNameAndNoNameActive));
        OnPropertyChanged(nameof(AreMultipleNamesAndNoNameActive));
        OnPropertyChanged(nameof(ActiveNamesCount));
        OnPropertyChanged(nameof(IsMixedFilteringActive));
    }
    
    public void AddName(string name)
    {
        ArgumentException.ThrowIfNullOrEmpty(name);

        if (!_names.Add(name))
            return;
        
        var selection = new NamedFilterSelection(name);
        _selections.Insert(_selections.Count - 1, selection);
    }

    public void RemoveName(string name)
    {
        var selection = _selections.OfType<NamedFilterSelection>()
            .FirstOrDefault(s => s.Name == name);

        if (selection is null)
            return;
        
        _selections.Remove(selection);
        _names.Remove(name);
    }
    
    public void ClearNames()
    {
        _names.Clear();
        
        _selections.Clear();
        _selections.Add(_noNameSelection);
    }
}