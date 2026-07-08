using System.Threading.Tasks;

namespace AvaloniaApplication1.Services.Overlay;

public interface IOverlayContent;

public interface IOverlayContent<TResult> : IOverlayContent
{
    Task<TResult> Result { get; }
}