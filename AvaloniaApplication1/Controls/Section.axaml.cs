using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Controls;

public class Section : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty = 
        AvaloniaProperty.Register<Section, string>(nameof(Title));

    public static readonly StyledProperty<bool> IsSeparatorVisibleProperty =
        AvaloniaProperty.Register<Section, bool>(nameof(IsSeparatorVisible));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
    
    public bool IsSeparatorVisible
    {
        get => GetValue(IsSeparatorVisibleProperty);
        set => SetValue(IsSeparatorVisibleProperty, value);
    }
}
