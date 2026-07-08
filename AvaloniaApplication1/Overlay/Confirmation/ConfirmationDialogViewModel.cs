using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Confirmation;

public partial class ConfirmationDialogViewModel(string title, string message, string action) : ViewModelBase, IOverlayContent<bool>
{
    public string Title { get; set; } = title;

    public string Message { get; } = message;

    public string Action { get; } = action;

    public Task<bool> Result => _result.Task;
    
    private readonly TaskCompletionSource<bool> _result = new();
    
    [RelayCommand]
    private void Confirm() => _result.SetResult(true);
    
    [RelayCommand]
    private void Cancel() => _result.SetResult(false);
}