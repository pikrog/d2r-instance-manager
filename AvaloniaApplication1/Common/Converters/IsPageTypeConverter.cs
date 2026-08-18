using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaApplication1.Navigation;

namespace AvaloniaApplication1.Common.Converters;

public class IsPageTypeConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not PageViewModel page || parameter is not Type pageType)
            return false;
        return page.GetType() == pageType;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}