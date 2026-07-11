using System.Windows.Input;

namespace AvaloniaApplication1.Overlay.Dialog.ConfirmDelete;

public interface IConfirmDeleteActions
{
    ICommand DeleteCommand { get; }
    ICommand CancelCommand { get; }
}