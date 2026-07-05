using System;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace AvaloniaApplication1.ViewModels.Card;

public sealed class CardCollection<T> : IDisposable where T : CardViewModel
{
    private readonly ObservableCollection<CardViewModel> _cards = [];

    public ReadOnlyObservableCollection<CardViewModel> Cards { get; }
    
    private readonly ObservableCollection<T> _entityCards;

    public CardCollection(ObservableCollection<T> entityCards)
    {
        _entityCards = entityCards;
        Cards = new ReadOnlyObservableCollection<CardViewModel>(_cards);

        Reset();

        _entityCards.CollectionChanged += OnEntityCardsChanged;
    }

    private void Reset()
    {
        _cards.Clear();
        foreach (var card in _entityCards)
            _cards.Add(card);
        _cards.Add(AddCardViewModel.Instance);
    }

    private void OnEntityCardsChanged(object? sender, NotifyCollectionChangedEventArgs e)
    {
        switch (e.Action)
        {
            case NotifyCollectionChangedAction.Add:
                var index = e.NewStartingIndex;
                foreach (T item in e.NewItems!)
                    _cards.Insert(index++, item);
                break;
            case NotifyCollectionChangedAction.Remove:
                foreach (T item in e.OldItems!)
                    _cards.Remove(item);
                break;
            case NotifyCollectionChangedAction.Replace:
                var replaceIndex = e.NewStartingIndex;
                foreach (T item in e.NewItems!)
                    _cards[replaceIndex++] = item;
                break;
            case NotifyCollectionChangedAction.Move:
                _cards.Move(e.OldStartingIndex, e.NewStartingIndex);
                break;
            case NotifyCollectionChangedAction.Reset:
                Reset();
                break;
        }
    }

    public void Dispose()
    {
        _entityCards.CollectionChanged -= OnEntityCardsChanged;
    }
}