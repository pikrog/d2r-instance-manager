using System.Threading.Tasks;
using System.Windows.Input;
using D2RInstanceManager.Common;
using CommunityToolkit.Mvvm.Input;

namespace D2RInstanceManager.Overlay.Dialog.Delete;

public abstract partial class DeleteDialogViewModelBase : ViewModelBase, IOverlayContent<bool>, IDeleteDialogActions
{
    public Task<bool> Result => _result.Task;
    
    private readonly TaskCompletionSource<bool> _result = new();
    
    private void Delete() => _result.SetResult(true);
    
    private void Cancel() => _result.SetResult(false);

    public ICommand DeleteCommand => new RelayCommand(Delete);
    
    public ICommand CancelCommand => new RelayCommand(Cancel);
}