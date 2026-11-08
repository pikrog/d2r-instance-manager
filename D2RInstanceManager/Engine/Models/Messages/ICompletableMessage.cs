using System.Threading.Tasks;

namespace D2RInstanceManager.Engine.Models.Messages;

public interface ICompletableMessage
{
    TaskCompletionSource Completion { get; }
}