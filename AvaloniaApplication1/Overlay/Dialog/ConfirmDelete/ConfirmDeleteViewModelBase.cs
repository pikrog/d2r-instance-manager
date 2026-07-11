using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Dialog.ConfirmDelete;

public abstract partial class ConfirmDeleteViewModelBase : ViewModelBase, IOverlayContent<bool>, IConfirmDeleteActions
{
    public Task<bool> Result => _result.Task;
    
    private readonly TaskCompletionSource<bool> _result = new();
    
    private void Confirm() => _result.SetResult(true);
    
    private void Cancel() => _result.SetResult(false);

    public ICommand DeleteCommand => new RelayCommand(Confirm);
    
    public ICommand CancelCommand => new RelayCommand(Cancel);
}