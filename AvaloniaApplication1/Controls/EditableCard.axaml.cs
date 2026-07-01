using System;
using System.Windows.Input;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.Metadata;
using Avalonia.Data;

namespace AvaloniaApplication1.Controls;

[PseudoClasses(Card.DimmedPseudoClass, EditingPseudoClass)]
public partial class EditableCard : UserControl
{
    private const string EditingPseudoClass = ":editing";

    public static readonly StyledProperty<bool> IsDimmedProperty =
        Card.IsDimmedProperty.AddOwner<EditableCard>();

    public static readonly StyledProperty<bool> IsEditedProperty =
        AvaloniaProperty.Register<EditableCard, bool>(nameof(IsEdited), defaultBindingMode: BindingMode.TwoWay);

    public static readonly StyledProperty<object?> ViewContentProperty =
        AvaloniaProperty.Register<EditableCard, object?>(nameof(ViewContent));

    public static readonly StyledProperty<object?> EditContentProperty =
        AvaloniaProperty.Register<EditableCard, object?>(nameof(EditContent));

    public static readonly StyledProperty<string> TitleProperty =
        AvaloniaProperty.Register<EditableCard, string>(nameof(Title));

    public static readonly StyledProperty<string> SubtitleProperty =
        AvaloniaProperty.Register<EditableCard, string>(nameof(Subtitle));

    public static readonly StyledProperty<ICommand> EditCommandProperty =
        AvaloniaProperty.Register<EditableCard, ICommand>(nameof(EditCommand));

    public static readonly StyledProperty<object> EditCommandParameterProperty =
        AvaloniaProperty.Register<EditableCard, object>(nameof(EditCommandParameter));

    public static readonly StyledProperty<ICommand> DeleteCommandProperty = 
        AvaloniaProperty.Register<EditableCard, ICommand>(nameof(DeleteCommand));

    public static readonly StyledProperty<object> DeleteCommandParameterProperty =
        AvaloniaProperty.Register<EditableCard, object>(nameof(DeleteCommandParameter));

    public static readonly StyledProperty<ICommand> AcceptCommandProperty = 
        AvaloniaProperty.Register<EditableCard, ICommand>(nameof(AcceptCommand));

    public static readonly StyledProperty<object> AcceptCommandParameterProperty =
        AvaloniaProperty.Register<EditableCard, object>(nameof(AcceptCommandParameter));
    
    public static readonly StyledProperty<ICommand> DiscardCommandProperty =
        AvaloniaProperty.Register<EditableCard, ICommand>(nameof(DiscardCommand));

    public static readonly StyledProperty<object> DiscardCommandParameterProperty =
        AvaloniaProperty.Register<EditableCard, object>(nameof(DiscardCommandParameter));

    public bool IsDimmed
    {
        get => GetValue(IsDimmedProperty);
        set => SetValue(IsDimmedProperty, value);
    }

    public bool IsEdited
    {
        get => GetValue(IsEditedProperty);
        set => SetValue(IsEditedProperty, value);
    }

    public object? ViewContent
    {
        get => GetValue(ViewContentProperty);
        set => SetValue(ViewContentProperty, value);
    }

    public object? EditContent
    {
        get => GetValue(EditContentProperty);
        set => SetValue(EditContentProperty, value);
    }

    public string Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public string Subtitle
    {
        get => GetValue(SubtitleProperty);
        set => SetValue(SubtitleProperty, value);
    }

    public ICommand EditCommand
    {
        get => GetValue(EditCommandProperty);
        set => SetValue(EditCommandProperty, value);
    }
    
    public object EditCommandParameter
    {
        get => GetValue(EditCommandParameterProperty);
        set => SetValue(EditCommandParameterProperty, value);
    }

    public ICommand DeleteCommand
    {
        get => GetValue(DeleteCommandProperty);
        set => SetValue(DeleteCommandProperty, value);
    }
    
    public object DeleteCommandParameter
    {
        get => GetValue(DeleteCommandParameterProperty);
        set => SetValue(DeleteCommandParameterProperty, value);
    }

    public ICommand AcceptCommand
    {
        get => GetValue(AcceptCommandProperty);
        set => SetValue(AcceptCommandProperty, value);
    }

    public object AcceptCommandParameter
    {
        get => GetValue(AcceptCommandParameterProperty);
        set => SetValue(AcceptCommandParameterProperty, value);
    }
    
    public ICommand DiscardCommand
    {
        get => GetValue(DiscardCommandProperty);
        set => SetValue(DiscardCommandProperty, value);
    }

    public object DiscardCommandParameter
    {
        get => GetValue(DiscardCommandParameterProperty);
        set => SetValue(DiscardCommandParameterProperty, value);
    }

    public EditableCard()
    {
        InitializeComponent();

        this.GetObservable(IsDimmedProperty).Subscribe(isDimmed => PseudoClasses.Set(Card.DimmedPseudoClass, isDimmed));
        this.GetObservable(IsEditedProperty).Subscribe(isEdited => PseudoClasses.Set(EditingPseudoClass, isEdited));
    }
}