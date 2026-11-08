using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace D2RInstanceManager.Controls;

public partial class FormField : UserControl
{
    public static readonly StyledProperty<string?> LabelProperty 
        = AvaloniaProperty.Register<FormField, string?>(nameof(Label));
    
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<FormField, string?>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true
            );

    public static readonly StyledProperty<bool> IsSecretProperty =
        AvaloniaProperty.Register<FormField, bool>(nameof(IsSecret));
    
    public static readonly StyledProperty<bool> RevealPasswordProperty =
        TextBox.RevealPasswordProperty.AddOwner<FormField>();
    
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }

    public bool IsSecret
    {
        get => GetValue(IsSecretProperty);
        set => SetValue(IsSecretProperty, value);
    }

    public FormField()
    {
        InitializeComponent();
    }
}