using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using D2RInstanceManager.Semantic;

namespace D2RInstanceManager.Controls;

[PseudoClasses(ActivePseudoClass)]
public class NavTab : Button
{
    private const string ActivePseudoClass = ":active";
    
    public static readonly StyledProperty<string?> SvgPathProperty = 
        AvaloniaProperty.Register<NavTab, string?>(nameof(SvgPath));
    
    public static readonly StyledProperty<bool> IsActiveProperty =
        AvaloniaProperty.Register<NavTab, bool>(nameof(IsActive));

    public static readonly StyledProperty<SemanticType> BadgeTypeProperty =
        AvaloniaProperty.Register<NavTab, SemanticType>(nameof(BadgeType));

    public static readonly StyledProperty<int> BadgeCountProperty =
        AvaloniaProperty.Register<NavTab, int>(nameof(BadgeCount));

    public string? SvgPath
    {
        get => GetValue(SvgPathProperty); 
        set => SetValue(SvgPathProperty, value);
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