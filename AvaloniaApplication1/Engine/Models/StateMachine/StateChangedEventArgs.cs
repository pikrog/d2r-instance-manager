using System;

namespace D2RInstanceManager.Engine.Models.StateMachine;

public class StateChangedEventArgs(RuntimeSnapshot snapshot) : EventArgs
{
    public RuntimeSnapshot Snapshot { get; } = snapshot;
}