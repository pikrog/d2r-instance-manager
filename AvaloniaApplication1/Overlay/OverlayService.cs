using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Overlay;

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