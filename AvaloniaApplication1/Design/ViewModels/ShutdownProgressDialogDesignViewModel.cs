using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Engine.Models;
using D2RInstanceManager.Overlay.Dialog.ShutdownProgress;

namespace D2RInstanceManager.Design.ViewModels;

public class ShutdownProgressDialogDesignViewModel() : ShutdownProgressDialogViewModel([
    new ShutdownRequest(Task.CompletedTask, true),
    new ShutdownRequest(Task.CompletedTask, false),
    new ShutdownRequest(Task.Delay(Timeout.InfiniteTimeSpan), true)
]);