using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Linq;
using CommunityToolkit.Mvvm.ComponentModel;

namespace AvaloniaApplication1.Selectable;

public sealed class SelectedItemsCollection<T> : ObservableObject, INotifyCollectionChanged where T : SelectableViewModelBase
{
    private readonly ReadOnlyObservableCollection<T> _allItems;
    
    private readonly ObservableCollection<T> _selectedItems;

    public ReadOnlyObservableCollection<T> Items { get; }
    
    public int Count => _selectedItems.Count;
    
    public bool Any => Count > 0;
    
    public event NotifyCollectionChangedEventHandler? CollectionChanged;
    
    public event EventHandler<ItemPropertyChangedEventArgs<T>>? ItemPropertyChanged;

    public SelectedItemsCollection(ReadOnlyObservableCollection<T> allItems)
    {
        _allItems = allItems;
        
        foreach (var item in allItems)
            item.PropertyChanged += OnItemPropertyChanged;
        
        _selectedItems = new ObservableCollection<T>(allItems.Where(i => i.IsSelected));
        Items = new ReadOnlyObservableCollection<T>(_selectedItems);

        ((INotifyCollectionChanged)allItems).CollectionChanged += OnAllItemsCollectionChanged;
        _selectedItems.CollectionChanged += OnSelectedItemsCollectionChanged;
    }

    private void OnSelectedItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        CollectionChanged?.Invoke(this, e);
        
        OnPropertyChanged(nameof(Count));
        OnPropertyChanged(nameof(Any));
    }

    private void OnAllItemsCollectionChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        if (e.Action == NotifyCollectionChangedAction.Reset)
        {
            foreach (var item in _selectedItems)
                item.PropertyChanged -= OnItemPropertyChanged;
            
            _selectedItems.Clear();
            
            foreach (var item in (ReadOnlyObservableCollection<T>)sender!)
                item.PropertyChanged += OnItemPropertyChanged;
            
            foreach (var item in _allItems.Where(i => i.IsSelected))
                _selectedItems.Add(item);
            
            return;
        }
        
        if (e.Action == NotifyCollectionChangedAction.Move)
            return;
        
        if (e.OldItems is not null)
        {
            foreach (T item in e.OldItems)
            {
                item.PropertyChanged -= OnItemPropertyChanged;
                _selectedItems.Remove(item);
            }
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
    }

    private void OnItemPropertyChanged(object? sender, PropertyChangedEventArgs e)
    {
        var item = (T)sender!;
        
        var itemPropertyChangedEventArgs = new ItemPropertyChangedEventArgs<T>(item, e.PropertyName);
        ItemPropertyChanged?.Invoke(this, itemPropertyChangedEventArgs);
        
        if (e.PropertyName != nameof(SelectableViewModelBase.IsSelected))
            return;
        
        if (item.IsSelected)
        {
            if (!_selectedItems.Contains(item))
                _selectedItems.Add(item);
        }
        else
            _selectedItems.Remove(item);
    }
}