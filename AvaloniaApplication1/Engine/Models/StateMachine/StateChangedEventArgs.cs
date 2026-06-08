using System;

namespace AvaloniaApplication1.Engine.Models.StateMachine;

public class StateChangedEventArgs(RuntimeSnapshot snapshot) : EventArgs
{
    public RuntimeSnapshot Snapshot { get; } = snapshot;
}