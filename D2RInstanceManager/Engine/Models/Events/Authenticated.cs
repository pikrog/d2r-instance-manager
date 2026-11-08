namespace D2RInstanceManager.Engine.Models.Events;

public sealed record Authenticated(string Token) : Event;