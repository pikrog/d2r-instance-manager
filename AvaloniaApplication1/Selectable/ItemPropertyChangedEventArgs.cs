using System;

namespace D2RInstanceManager.Selectable;

public class ItemPropertyChangedEventArgs<T>(T item, string? propertyName) : EventArgs
    where T : class
{
    public T Item { get; } = item;

    public string? PropertyName { get; } = propertyName;
}