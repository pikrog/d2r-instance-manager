namespace D2RInstanceManager.Engine.Models.Events;

public abstract record ErrorEvent : Event
{
    public abstract string Message { get; }
}