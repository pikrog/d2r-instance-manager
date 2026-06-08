namespace AvaloniaApplication1.Engine.Models.Events.Authentication;

public sealed record Authenticated(string Token) : Event;