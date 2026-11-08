using System.Threading.Tasks;
using D2RInstanceManager.Common;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace D2RInstanceManager.Overlay.Dialog.SetPassphrase;

public partial class SetPassphraseDialogViewModel : ViewModelBase, IOverlayContent<SetPassphraseResult>
{
    public Task<SetPassphraseResult> Result => _result.Task;
    
    private readonly TaskCompletionSource<SetPassphraseResult> _result = new();
    
    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetPassphraseCommand))]
    public partial string Passphrase { get; set; } = string.Empty;

    [ObservableProperty]
    [NotifyCanExecuteChangedFor(nameof(SetPassphraseCommand))]
    public partial string ConfirmPassphrase { get; set; } = string.Empty;
    
    public bool CanSetPassphrase => !string.IsNullOrEmpty(Passphrase) && Passphrase == ConfirmPassphrase;
    
    [RelayCommand(CanExecute = nameof(CanSetPassphrase))]
    private void SetPassphrase() => _result.SetResult(new SetPassphraseResult.Entered(Passphrase));
    
    [RelayCommand]
    private void Cancel() => _result.SetResult(SetPassphraseResult.Canceled.Instance);
}