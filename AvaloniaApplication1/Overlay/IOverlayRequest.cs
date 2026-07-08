namespace AvaloniaApplication1.Overlay;

public interface IOverlayRequest<TResult>
{
    IOverlayContent<TResult> CreateContent();
};