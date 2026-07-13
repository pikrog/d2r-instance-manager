using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Dialog.DiscardChanges;

public partial class DiscardChangesDialogViewModel : ViewModelBase, IOverlayContent<bool>
{
    public Task<bool> Result => _result.Task;
    
    private readonly TaskCompletionSource<bool> _result = new();
    
    [RelayCommand]
    private void DiscardChanges() => _result.SetResult(true);
    
    [RelayCommand]
    private void Cancel() => _result.SetResult(false);
}