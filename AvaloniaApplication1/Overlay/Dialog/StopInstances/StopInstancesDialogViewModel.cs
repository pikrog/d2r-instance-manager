using System.Threading.Tasks;
using System.Windows.Input;
using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Dialog.StopInstances;

public class StopInstancesDialogViewModel(int instanceCount) : ViewModelBase, IOverlayContent<StopInstancesAction>
{
    public int InstanceCount { get; } = instanceCount;
    
    private readonly TaskCompletionSource<StopInstancesAction> _result = new();

    public Task<StopInstancesAction> Result => _result.Task;

    private void StopInstances() => _result.SetResult(StopInstancesAction.StopInstances);
    
    private void LeaveInstances() => _result.SetResult(StopInstancesAction.LeaveInstances);
    
    private void Cancel() => _result.SetResult(StopInstancesAction.Cancel);
    
    public ICommand StopInstancesCommand => new RelayCommand(StopInstances);
    
    public ICommand LeaveInstancesCommand => new RelayCommand(LeaveInstances);

    public ICommand CancelCommand => new RelayCommand(Cancel);
}