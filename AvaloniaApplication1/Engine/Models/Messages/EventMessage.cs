using AvaloniaApplication1.Engine.Models.Events;

namespace AvaloniaApplication1.Engine.Models.Messages;

public record EventMessage(Event Event) : Message;