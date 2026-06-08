using System;
using System.Globalization;
using Avalonia.Data.Converters;
using Avalonia.Media;
using AvaloniaApplication1.Engine.Models;

namespace AvaloniaApplication1.Converters;

public class GameInstanceStatusToBorderBrushConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (!targetType.IsAssignableTo(typeof(IBrush)))
            throw new NotSupportedException("Target type must be assignable to IBrush");
        
        if (value is not GameInstanceStatus status)
            return null;
        
        return status switch
        {
            GameInstanceStatus.Inactive => Brushes.Gray,
            GameInstanceStatus.QueuedForStart => Brushes.Orange,
            GameInstanceStatus.Unlocking => Brushes.Orange,
            GameInstanceStatus.Running => Brushes.Green,
            GameInstanceStatus.Closing => Brushes.Orange,
            GameInstanceStatus.Exited => Brushes.Gray,
            GameInstanceStatus.ExitedPrematurely => Brushes.Red,
            GameInstanceStatus.Terminated => Brushes.Red,
            GameInstanceStatus.Timeout => Brushes.Red,
            GameInstanceStatus.Failed => Brushes.Red,
            GameInstanceStatus.Unknown => Brushes.Red,
            _ => null
        };
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotSupportedException();
    }
}