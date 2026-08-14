using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models;
using AvaloniaApplication1.Overlay.Dialog.ShutdownProgress;

namespace AvaloniaApplication1.Design.ViewModels;

public class ShutdownProgressDialogDesignViewModel() : ShutdownProgressDialogViewModel([
    new ShutdownRequest(Task.CompletedTask, true),
    new ShutdownRequest(Task.CompletedTask, false),
    new ShutdownRequest(Task.Delay(Timeout.InfiniteTimeSpan), true)
]);