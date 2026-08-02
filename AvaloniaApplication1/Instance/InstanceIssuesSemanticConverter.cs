using System;
using System.Globalization;
using Avalonia;
using Avalonia.Data.Converters;
using AvaloniaApplication1.Instance.ViewModels;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Instance;

public class InstanceIssuesSemanticConverter : IValueConverter
{
    public object? Convert(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        if (value is not InstanceItemViewModel instance || instance.IsActive || !instance.HasIssues)
            return AvaloniaProperty.UnsetValue;
        
        return instance.RequiresAttention ? SemanticType.Danger : SemanticType.Warning;
    }

    public object? ConvertBack(object? value, Type targetType, object? parameter, CultureInfo culture)
    {
        throw new NotImplementedException();
    }
}