namespace AvaloniaApplication1.Engine.Models.StateMachine;

public enum State
{
    Inactive,
    Authenticating,
    WaitingForStart,
    Starting,
    WaitingForUnlock,
    Running,
    Stopping,
}