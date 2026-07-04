using System;
using System.Globalization;
using Avalonia.Data.Converters;
using AvaloniaApplication1.Models;

namespace AvaloniaApplication1.Converters;

public class GameInstanceStatusToSpinnerOpacityConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!targetType.IsAssignableTo(typeof(double)))
            throw new NotSupportedException("Target type must be assignable to double");

        if (value is GameInstanceStatus status)
            return status is GameInstanceStatus.Authenticating
                or GameInstanceStatus.Starting
                or GameInstanceStatus.Unlocking
                or GameInstanceStatus.Stopping ? 1.0 : 0.0;
                //or GameInstanceStatus.QueuedForStart;
        
        return 0.0;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}