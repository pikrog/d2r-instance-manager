using System;
using System.Threading.Tasks;

namespace D2RInstanceManager.Overlay;

public class OverlayService(IOverlayController controller)
{
    public async Task<TResult> ShowAsync<TResult>(IOverlayContent<TResult> content)
    {
        if (controller.OverlayContent is not null)
            throw new InvalidOperationException("Overlay is already shown");
        
        controller.OverlayContent = content;
        try
        {
            return await content.Result;
        }
        finally
        {
            controller.OverlayContent = null;
        }
    }
}