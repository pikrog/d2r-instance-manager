using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Media;
using AvaloniaApplication1.Semantic;

namespace AvaloniaApplication1.Controls;

[PseudoClasses(ActivePseudoClass)]
public class NavTab : Button
{
    private const string ActivePseudoClass = ":active";
    
    public static readonly StyledProperty<IImage?> IconProperty = 
        AvaloniaProperty.Register<NavTab, IImage?>(nameof(Icon));
    
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<NavTab, bool>(nameof(IsActive));

    public static readonly StyledProperty<SemanticType> BadgeTypeProperty =
        AvaloniaProperty.Register<NavTab, SemanticType>(nameof(BadgeType));

    public static readonly StyledProperty<int> BadgeCountProperty =
        AvaloniaProperty.Register<NavTab, int>(nameof(BadgeCount));

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

    public SemanticType BadgeType
    {
        get => GetValue(BadgeTypeProperty);
        set => SetValue(BadgeTypeProperty, value);
    }

    public int BadgeCount
    {
        get => GetValue(BadgeCountProperty);
        set => SetValue(BadgeCountProperty, value);
    }

    static NavTab()
    {
        IsActiveProperty.Changed.AddClassHandler<NavTab>((c, e) =>
        {
            c.PseudoClasses.Set(ActivePseudoClass, e.NewValue is true);
        });
    }
}