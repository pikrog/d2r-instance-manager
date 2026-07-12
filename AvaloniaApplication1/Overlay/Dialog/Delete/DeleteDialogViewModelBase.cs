using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Dialog.Delete;

public abstract partial class DeleteDialogViewModelBase : ViewModelBase, IOverlayContent<bool>, IDeleteDialogActions
{
    public Task<bool> Result => _result.Task;
    
    private readonly TaskCompletionSource<bool> _result = new();
    
    private void Confirm() => _result.SetResult(true);
    
    private void Cancel() => _result.SetResult(false);

    public ICommand DeleteCommand => new RelayCommand(Confirm);
    
    public ICommand CancelCommand => new RelayCommand(Cancel);
}