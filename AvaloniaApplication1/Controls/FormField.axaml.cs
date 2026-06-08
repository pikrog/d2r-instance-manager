using Avalonia;
using Avalonia.Controls;
using Avalonia.Data;
using Avalonia.Markup.Xaml;

namespace AvaloniaApplication1.Controls;

public partial class FormField : UserControl
{
    #region Label
    public static readonly StyledProperty<string?> LabelProperty = AvaloniaProperty.Register<FormField, string?>(nameof(Label));
    
    public string? Label
    {
        get => GetValue(LabelProperty);
        set => SetValue(LabelProperty, value);
    }
    #endregion
    
    #region Text
    public static readonly StyledProperty<string?> TextProperty =
        AvaloniaProperty.Register<FormField, string?>(
            nameof(Text),
            defaultBindingMode: BindingMode.TwoWay,
            enableDataValidation: true);
    
    public string? Text
    {
        get => GetValue(TextProperty);
        set => SetValue(TextProperty, value);
    }
    #endregion
    
    public FormField()
    {
        InitializeComponent();
    }
}