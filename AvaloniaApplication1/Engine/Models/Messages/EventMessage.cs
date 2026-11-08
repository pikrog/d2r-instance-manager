using D2RInstanceManager.Engine.Models.Events;

namespace D2RInstanceManager.Engine.Models.Messages;

public record EventMessage(Event Event) : Message;