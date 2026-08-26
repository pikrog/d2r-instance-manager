using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using Avalonia.Input;
using AvaloniaApplication1.HotKey;

namespace AvaloniaApplication1.Common.Converters;

public sealed class KeyCombinationToKeyGestureConverter : IValueConverter
{
    public object Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is KeyCombination keyCombination
            ? new KeyGesture(keyCombination.Key, keyCombination.KeyModifiers)
            : AvaloniaProperty.UnsetValue;
    }

    public object ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        return value is KeyGesture keyGesture
            ? new KeyCombination(keyGesture.Key, keyGesture.KeyModifiers)
            : AvaloniaProperty.UnsetValue;
    }
}