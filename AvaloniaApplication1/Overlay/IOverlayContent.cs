using System.Threading.Tasks;

namespace D2RInstanceManager.Overlay;

public interface IOverlayContent;

public interface IOverlayContent<TResult> : IOverlayContent
{
    Task<TResult> Result { get; }
}