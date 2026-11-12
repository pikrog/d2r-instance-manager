namespace D2RInstanceManager.Engine.Models.Events;

public sealed record AuthenticationFailed : ErrorEvent
{
    public override string Message => "Authentication failed";
}