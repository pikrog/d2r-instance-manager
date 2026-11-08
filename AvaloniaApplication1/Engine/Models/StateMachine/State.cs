namespace D2RInstanceManager.Engine.Models.StateMachine;

public enum State
{
    Inactive,
    Authenticating,
    WaitingForStart,
    Starting,
    WaitingForUnlock,
    Running,
    Stopping,
    Shutdown,
}