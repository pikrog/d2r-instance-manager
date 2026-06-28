using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Controls;

public class Section : ContentControl
{
    public static readonly StyledProperty<string> TitleProperty 
        = AvaloniaProperty.Register<Section, string>(nameof(Title));

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }
}
