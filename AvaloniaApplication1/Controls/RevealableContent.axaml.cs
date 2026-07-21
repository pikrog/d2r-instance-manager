using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Controls.Primitives;

namespace AvaloniaApplication1.Controls;

[PseudoClasses(RevealedPseudoClass)]
public class RevealableContent : ContentControl
{
    private const string RevealedPseudoClass = ":revealed";
    
    public static readonly StyledProperty<bool> IsRevealedProperty =
        AvaloniaProperty.Register<RevealableContent, bool>(nameof(IsRevealed));
    
    public bool IsRevealed
    {
        get => GetValue(IsRevealedProperty);
        set => SetValue(IsRevealedProperty, value);
    }

    static RevealableContent()
    {
        IsRevealedProperty.Changed.AddClassHandler<RevealableContent>((c, e) =>
        {
            c.PseudoClasses.Set(RevealedPseudoClass, e.NewValue is true);
        });
    }
}