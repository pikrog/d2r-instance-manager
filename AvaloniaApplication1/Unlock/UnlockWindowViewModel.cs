using System;
using System.Threading.Tasks;
using D2RInstanceManager.Common;
using D2RInstanceManager.Secret;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace D2RInstanceManager.Unlock;

public partial class UnlockWindowViewModel(IUnlockService unlockService) : ViewModelBase
{
    public Task<bool> IsUnlocked => _result.Task;

    private readonly TaskCompletionSource<bool> _result = new(TaskCreationOptions.RunContinuationsAsynchronously);

    public event EventHandler? UnlockFailed;

    [ObservableProperty]
    public partial string Passphrase { get; set; } = string.Empty;
    
    [ObservableProperty]
    public partial bool FailedToUnlock { get; set; }
    
    private void Complete(bool result) => _result.TrySetResult(result);

    [RelayCommand]
    private void Unlock()
    {
        if (!unlockService.TryUnlock(Passphrase))
        {
            FailedToUnlock = true;
            UnlockFailed?.Invoke(this, EventArgs.Empty);
            return;
        }
        
        Complete(true);
    }

    [RelayCommand]
    private void Cancel() => Complete(false);
    
    public void Close() => Complete(false);
}