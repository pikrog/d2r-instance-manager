using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Helpers;

public static class MultiboxUnlocker
{
    public enum UnlockResult
    {
        Success,
        EventNotFound,
        CloseSourceFailed,
    }

    private const string EventObjectTypeName = "Event";
    private const string EventObjectFileNameSuffix = "DiabloII Check For Other Instances";

    public static async Task<UnlockResult> UnlockAsync(CancellationToken cancellationToken = default)
    {
        var @event = await Task.Run(() => KernelObject.GetAll(
            t => t.Equals(EventObjectTypeName),
            n => n.EndsWith(EventObjectFileNameSuffix)
        ).FirstOrDefault(), cancellationToken);

        if (@event is null)
            return UnlockResult.EventNotFound;

        var closeSourceResult = @event.CloseSource();
        return closeSourceResult == KernelObject.CloseResult.Success 
            ? UnlockResult.Success 
            : UnlockResult.CloseSourceFailed; 
    }
}