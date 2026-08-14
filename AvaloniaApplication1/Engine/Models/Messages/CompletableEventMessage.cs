using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Models.Messages;

public sealed record CompletableEventMessage(Event Event, TaskCompletionSource Completion) : EventMessage(Event), ICompletableMessage;