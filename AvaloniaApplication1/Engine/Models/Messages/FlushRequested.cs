using System.Threading.Tasks;

namespace AvaloniaApplication1.Engine.Models.Messages;

public sealed record FlushRequested(TaskCompletionSource Completion) : Message;