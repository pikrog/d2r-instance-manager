using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;
using Avalonia.Data;
using Avalonia.Media;

namespace AvaloniaApplication1.Controls;

[PseudoClasses(ActivePseudoClass)]
public class NavTab : Button
{
    private const string ActivePseudoClass = ":active";
    
    public static readonly StyledProperty<IImage?> IconProperty = 
        AvaloniaProperty.Register<NavTab, IImage?>(nameof(Icon));
    
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<NavTab, bool>(nameof(IsActive));

    public IImage? Icon
    {
        get => GetValue(IconProperty); 
        set => SetValue(IconProperty, value);
    }

    public bool IsActive
    {
        get => GetValue(IsActiveProperty);
        set => SetValue(IsActiveProperty, value);
    }

    static NavTab()
    {
        IsActiveProperty.Changed.AddClassHandler<NavTab>((c, e) =>
        {
            c.PseudoClasses.Set(ActivePseudoClass, e.NewValue is true);
        });
    }
}