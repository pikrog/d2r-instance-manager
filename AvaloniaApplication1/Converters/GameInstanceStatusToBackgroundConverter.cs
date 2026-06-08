using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AvaloniaApplication1.Engine.Models;

namespace AvaloniaApplication1.Converters;

public class GameInstanceStatusToBackgroundConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!targetType.IsAssignableTo(typeof(IBrush)))
            throw new NotSupportedException("Target type must be assignable to IBrush");
        
        if (value is not GameInstanceStatus status)
            return null;
        
        return status switch
        {
            GameInstanceStatus.Inactive => Brushes.LightGray,
            GameInstanceStatus.QueuedForStart => Brushes.LightYellow,
            GameInstanceStatus.Unlocking => Brushes.LightYellow,
            GameInstanceStatus.Running => Brushes.LightGreen,
            GameInstanceStatus.Closing => Brushes.LightYellow,
            GameInstanceStatus.Exited => Brushes.LightGray,
            GameInstanceStatus.ExitedPrematurely => Brushes.LightCoral,
            GameInstanceStatus.Terminated => Brushes.LightCoral,
            GameInstanceStatus.Timeout => Brushes.LightCoral,
            GameInstanceStatus.Failed => Brushes.LightCoral,
            GameInstanceStatus.Unknown => Brushes.LightCoral,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}