using System;
using System.Collections.Generic;
using System.Globalization;
using Avalonia.Data.Converters;

namespace D2RInstanceManager.Common.Converters;

public class ReferenceEqualsConverter : IMultiValueConverter
{
    public object? Convert(IList<object?> values, Type targetType, object? parameter, CultureInfo culture)
    {
        return values.Count >= 2 && ReferenceEquals(values[0], values[1]);
    }
}
