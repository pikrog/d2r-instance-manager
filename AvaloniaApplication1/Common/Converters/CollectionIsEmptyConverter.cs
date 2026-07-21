using System;
using System.Collections;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaApplication1.Common.Converters;

public class CollectionIsEmptyConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not ICollection collection)
            return false;
        
        return collection.Count == 0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}