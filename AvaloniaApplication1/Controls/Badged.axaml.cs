using Avalonia;
using Avalonia.Controls;
using Avalonia.Media;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Controls;

public class Badged : ContentControl
{
    public static readonly StyledProperty<SemanticType> TypeProperty =
        AvaloniaProperty.Register<Badged, SemanticType>(nameof(Type));

    public static readonly StyledProperty<int> CountProperty =
        AvaloniaProperty.Register<Badged, int>(nameof(Count));
    
    public static readonly StyledProperty<double> OverlayOffsetXProperty =
        AvaloniaProperty.Register<Badged, double>(nameof(OverlayOffsetX));

    public static readonly StyledProperty<double> OverlayOffsetYProperty =
        AvaloniaProperty.Register<Badged, double>(nameof(OverlayOffsetY));

    public static readonly StyledProperty<IBrush?> BadgeBackgroundProperty =
        AvaloniaProperty.Register<Badged, IBrush?>(nameof(BadgeBackground));

    public static readonly StyledProperty<IBrush?> BadgeForegroundProperty =
        AvaloniaProperty.Register<Badged, IBrush?>(nameof(BadgeForeground));
    
    public SemanticType Type
    {
        get => GetValue(TypeProperty);
        set => SetValue(TypeProperty, value);
    }

    public int Count
    {
        get => GetValue(CountProperty);
        set => SetValue(CountProperty, value);
    }
    
    public double OverlayOffsetX
    {
        get => GetValue(OverlayOffsetXProperty);
        set => SetValue(OverlayOffsetXProperty, value);
    }

    public double OverlayOffsetY
    {
        get => GetValue(OverlayOffsetYProperty); 
        set => SetValue(OverlayOffsetYProperty, value);
    }
    
    public IBrush? BadgeBackground
    {
        get => GetValue(BadgeBackgroundProperty);
        set => SetValue(BadgeBackgroundProperty, value);
    }

    public IBrush? BadgeForeground
    {
        get => GetValue(BadgeForegroundProperty);
        set => SetValue(BadgeForegroundProperty, value);
    }
}