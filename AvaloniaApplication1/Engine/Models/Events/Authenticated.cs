namespace AvaloniaApplication1.Engine.Models.Events;

public sealed record Authenticated(string Token) : Event;