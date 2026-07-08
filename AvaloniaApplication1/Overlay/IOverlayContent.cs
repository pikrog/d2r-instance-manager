using System.Threading.Tasks;

namespace AvaloniaApplication1.Overlay;

public interface IOverlayContent;

public interface IOverlayContent<TResult> : IOverlayContent
{
    Task<TResult> Result { get; }
}