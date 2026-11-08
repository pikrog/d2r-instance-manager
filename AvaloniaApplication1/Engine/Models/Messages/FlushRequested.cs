using System.Threading.Tasks;

namespace D2RInstanceManager.Engine.Models.Messages;

public sealed record FlushRequested(TaskCompletionSource Completion) : Message, ICompletableMessage;