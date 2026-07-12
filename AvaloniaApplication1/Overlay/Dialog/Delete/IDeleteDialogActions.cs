using System.Windows.Input;

namespace AvaloniaApplication1.Overlay.Dialog.Delete;

public interface IDeleteDialogActions
{
    ICommand DeleteCommand { get; }
    ICommand CancelCommand { get; }
}