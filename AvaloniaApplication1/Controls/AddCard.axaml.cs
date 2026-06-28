using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Controls;

public partial class AddCard : UserControl
{
    public static readonly StyledProperty<bool> IsDimmedProperty =
        Card.IsDimmedProperty.AddOwner<AddCard>();
    
    public static readonly StyledProperty<ICommand> CommandProperty =
        AvaloniaProperty.Register<AddCard, ICommand>(nameof(Command));

    public bool IsDimmed
    {
        get => GetValue(IsDimmedProperty);
        set => SetValue(IsDimmedProperty, value);
    }

    public ICommand Command
    {
        get => GetValue(CommandProperty);
        set => SetValue(CommandProperty, value);
    }

    public AddCard()
    {
        InitializeComponent();
        
        this.GetObservable(IsDimmedProperty).Subscribe(isDimmed => PseudoClasses.Set(Card.DimmedPseudoClass, isDimmed));
    }
}