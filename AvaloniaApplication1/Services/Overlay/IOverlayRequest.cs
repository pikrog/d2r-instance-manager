namespace AvaloniaApplication1.Services.Overlay;

public interface IOverlayRequest<TResult>
{
    IOverlayContent<TResult> CreateContent();
};