using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Overlay;

public class OverlayService(IOverlayHost host)
{
    public async Task<TResult> ShowAsync<TResult>(IOverlayContent<TResult> content)
    {
        if (host.OverlayContent is not null)
            throw new InvalidOperationException("Overlay is already shown");
        
        host.OverlayContent = content;
        try
        {
            return await content.Result;
        }
        finally
        {
            host.OverlayContent = null;
        }
    }
}