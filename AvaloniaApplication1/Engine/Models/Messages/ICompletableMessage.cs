using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine.Models.Messages;

public interface ICompletableMessage
{
    TaskCompletionSource Completion { get; }
}