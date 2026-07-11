using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Overlay.Dialog.ConfirmDelete;

public partial class ConfirmDeleteDialog : UserControl
{
    public static readonly StyledProperty<Control?> BodyProperty = 
        Controls.Dialog.BodyProperty.AddOwner<ConfirmDeleteDialog>();
    
    public static readonly StyledProperty<string?> TitleProperty =
        AvaloniaProperty.Register<ConfirmDeleteDialog, string?>(nameof(Title));
    
    public static readonly StyledProperty<IConfirmDeleteActions?> ActionsProperty =
        AvaloniaProperty.Register<ConfirmDeleteDialog, IConfirmDeleteActions?>(nameof(Actions));

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

    public IConfirmDeleteActions? Actions
    {
        get => GetValue(ActionsProperty);
        set => SetValue(ActionsProperty, value);
    }
    
    public ConfirmDeleteDialog()
    {
        InitializeComponent();
    }
}