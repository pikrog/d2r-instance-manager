using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace AvaloniaApplication1.Converters;

public class ReferenceNotNullAndNotEqualsConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values is [not null, _, ..] && !ReferenceEquals(values[0], values[1]);
    }
}
