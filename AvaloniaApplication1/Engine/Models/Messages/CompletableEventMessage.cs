using System.Threading.Tasks;
using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Models.Messages;

public sealed record CompletableEventMessage(Event Event, TaskCompletionSource Completion) : EventMessage(Event), ICompletableMessage;