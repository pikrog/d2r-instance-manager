using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Selectable;

public sealed class SelectedItemsCollection<T> : ObservableObject where T : SelectableViewModelBase
{
    private readonly ObservableCollection<T> _selectedItems;

    public ReadOnlyObservableCollection<T> Items { get; }
    
    public int Count => _selectedItems.Count;
    
    public bool Any => Count > 0;

    public SelectedItemsCollection(ObservableCollection<T> allItems)
    {
        foreach (var item in allItems)
            item.PropertyChanged += OnItemPropertyChanged;
        
        _selectedItems = new ObservableCollection<T>(allItems.Where(i => i.IsSelected));
        Items = new ReadOnlyObservableCollection<T>(_selectedItems);

        allItems.CollectionChanged += OnOriginalItemsCollectionChanged;
        _selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
    }

    private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Any));
    }

    private void OnOriginalItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var item in _selectedItems)
                item.PropertyChanged -= OnItemPropertyChanged;
            _selectedItems.Clear();
            foreach (var item in (ObservableCollection<T>)sender!)
                item.PropertyChanged += OnItemPropertyChanged;
            return;
        }
        
        if (e.NewItems is not null)
        {
            foreach (T item in e.NewItems)
            {
                if (item.IsSelected)
                    _selectedItems.Add(item);
                item.PropertyChanged += OnItemPropertyChanged;
            }
        }

        if (e.OldItems is not null)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                _selectedItems.Remove(item);
            }
        }
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        if (e.PropertyName != nameof(SelectableViewModelBase.IsSelected))
            return;
        
        var item = (T)sender!;
        if (item.IsSelected)
        {
            if (!_selectedItems.Contains(item))
                _selectedItems.Add(item);
        }
        else
            _selectedItems.Remove(item);
    }
}