using System.Windows.Input;

namespace D2RInstanceManager.Overlay.Dialog.Delete;

public interface IDeleteDialogActions
{
    ICommand DeleteCommand { get; }
    ICommand CancelCommand { get; }
}