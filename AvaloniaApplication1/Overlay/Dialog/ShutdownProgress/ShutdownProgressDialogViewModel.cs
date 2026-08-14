using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.Engine.Models;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace AvaloniaApplication1.Overlay.Dialog.ShutdownProgress;

public partial class ShutdownProgressDialogViewModel : ViewModelBase, IOverlayContent<Unit>
{
    [ObservableProperty]
    public partial int CompletedCount { get; set; }
    
    public int TrackedShutdownTaskCount { get; }
    
    private readonly TaskCompletionSource _exitNowRequested = new();

    private readonly Task _completion;

    public Task<Unit> Result { get; }

    private async Task<Unit> WaitForCompletionAsync()
    {
        await Task.WhenAny(_completion, _exitNowRequested.Task);
        return Unit.Value;
    }

    private async Task ObserveShutdownTaskAsync(Task shutdownTask)
    {
        try
        {
            await shutdownTask;
        }
        finally
        {
            CompletedCount++;
        }
    }

    public ShutdownProgressDialogViewModel(IReadOnlyList<ShutdownRequest> shutdownTasks)
    {
        var observedTasks = shutdownTasks
            .Select(t => t.TrackProgress
                ? ObserveShutdownTaskAsync(t.Completion)
                : t.Completion)
            .ToList();

        TrackedShutdownTaskCount = shutdownTasks.Count(t => t.TrackProgress);

        _completion = Task.WhenAll(observedTasks);

        Result = WaitForCompletionAsync();
    }
    
    [RelayCommand]
    private void ExitNow() => _exitNowRequested.SetResult();
}