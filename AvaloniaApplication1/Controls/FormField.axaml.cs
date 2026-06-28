using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1.Controls;

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
    
    public static readonly StyledProperty<char> PasswordCharProperty =
        TextBox.PasswordCharProperty.AddOwner<FormField>();
    
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
    
    public char PasswordChar
    {
        get => GetValue(PasswordCharProperty);
        set => SetValue(PasswordCharProperty, value);
    }
    
    public bool RevealPassword
    {
        get => GetValue(RevealPasswordProperty);
        set => SetValue(RevealPasswordProperty, value);
    }
    
    public FormField()
    {
        InitializeComponent();
    }
}