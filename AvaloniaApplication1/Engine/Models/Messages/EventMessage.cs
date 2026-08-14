using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Models.Messages;

public sealed record EventMessage(Event Event) : Message;