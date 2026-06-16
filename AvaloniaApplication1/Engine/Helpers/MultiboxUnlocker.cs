using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Common;
using AvaloniaApplication1.Engine.Models.Errors;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

using MultiboxUnlockResult = Result<Unit, MultiboxUnlockError>;

public static class MultiboxUnlocker
{
    private const string EventObjectTypeName = "Event";
    private const string EventObjectFileNameSuffix = "DiabloII Check For Other Instances";

    public static async Task<MultiboxUnlockResult> UnlockAsync(CancellationToken cancellationToken = default)
    {
        var @event = await Task.Run(() => KernelObject.GetAll(
            t => t.Equals(EventObjectTypeName),
            n => n.EndsWith(EventObjectFileNameSuffix), 
            cancellationToken).FirstOrDefault(), cancellationToken);

        if (@event is null)
            return MultiboxUnlockResult.Failure(MultiboxUnlockError.EventNotFound);

        var closeSourceResult = @event.CloseSource();
        return closeSourceResult == KernelObject.CloseResult.Success 
            ? MultiboxUnlockResult.Success()
            : MultiboxUnlockResult.Failure(MultiboxUnlockError.CloseSourceFailed); 
    }
}