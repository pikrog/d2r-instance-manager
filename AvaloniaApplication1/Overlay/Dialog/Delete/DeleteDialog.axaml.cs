using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Overlay.Dialog.Delete;

public partial class DeleteDialog : UserControl
{
    public static readonly StyledProperty<Control?> BodyProperty = 
        Controls.Dialog.BodyProperty.AddOwner<DeleteDialog>();
    
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<DeleteDialog, string?>(nameof(Title));
    
    public static readonly StyledProperty<IDeleteDialogActions?> ActionsProperty =
        AvaloniaProperty.Register<DeleteDialog, IDeleteDialogActions?>(nameof(Actions));

    public string? Title
    {
        get => GetValue(TitleProperty);
        set => SetValue(TitleProperty, value);
    }

    public Control? Body
    {
        get => GetValue(BodyProperty); 
        set => SetValue(BodyProperty, value);
    }

    public IDeleteDialogActions? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }
    
    public DeleteDialog()
    {
        InitializeComponent();
    }
}