using System;
using System.Threading.Tasks;

namespace AvaloniaApplication1.Overlay;

public class OverlayService(IOverlayHost host)
{
    public async Task<TResult> ShowAsync<TResult>(IOverlayRequest<TResult> request)
    {
        if (host.OverlayContent is not null)
            throw new InvalidOperationException("Overlay is already shown");
        
        var content = request.CreateContent();
        host.OverlayContent = content;
        var result = await content.Result;
        host.OverlayContent = null;
        return result;
    }
}