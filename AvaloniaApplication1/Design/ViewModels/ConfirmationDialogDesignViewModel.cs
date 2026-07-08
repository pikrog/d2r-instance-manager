using AvaloniaApplication1.Services.Overlay.Confirmation;

namespace AvaloniaApplication1.Design.ViewModels;

public class ConfirmationDialogDesignViewModel() : ConfirmationDialogViewModel(
    "Confirm delete",
    "This region is used by 3 instances. Are you sure and want to delete?",
    "Delete"
    )
{
    
}