using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;

namespace AvaloniaApplication1.Card.Views;

[PseudoClasses(DimmedPseudoClass)]
public class Card : ContentControl
{
    internal const string DimmedPseudoClass = ":dimmed";
    
    public static readonly StyledProperty<bool> IsDimmedProperty 
        = AvaloniaProperty.Register<Card, bool>(nameof(IsDimmed), defaultBindingMode: BindingMode.TwoWay);

    public bool IsDimmed
    {
        get => GetValue(IsDimmedProperty);
        set => SetValue(IsDimmedProperty, value);
    }

    public Card()
    {
        this.GetObservable(IsDimmedProperty).Subscribe(isDimmed => PseudoClassesExtensions.Set(PseudoClasses, DimmedPseudoClass, isDimmed));
    }
}