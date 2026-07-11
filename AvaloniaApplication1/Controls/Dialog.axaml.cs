using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Controls;

public partial class Dialog : UserControl
{
    public static readonly StyledProperty<Control?> HeaderProperty 
        = AvaloniaProperty.Register<Dialog, Control?>(nameof(Header));
    
    public static readonly StyledProperty<Control?> BodyProperty 
        = AvaloniaProperty.Register<Dialog, Control?>(nameof(Body));
    
    public static readonly StyledProperty<Control?> FooterProperty 
        = AvaloniaProperty.Register<Dialog, Control?>(nameof(Footer));

    public Control? Header
    {
        get => GetValue(HeaderProperty); 
        set => SetValue(HeaderProperty, value);
    }

    public Control? Body
    {
        get => GetValue(BodyProperty); 
        set => SetValue(BodyProperty, value);
    }

    public Control? Footer
    {
        get => GetValue(FooterProperty); 
        set => SetValue(FooterProperty, value);
    }
    
    
    public Dialog()
    {
        InitializeComponent();
    }
}