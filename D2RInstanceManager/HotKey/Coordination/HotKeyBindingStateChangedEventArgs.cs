using System;

namespace D2RInstanceManager.HotKey.Coordination;

public class HotKeyBindingStateChangedEventArgs<THotKeyCommand>(THotKeyCommand command, HotKeyBindingState state) 
    : EventArgs
    where THotKeyCommand : HotKeyCommand
{
    public THotKeyCommand Command { get; } = command;
        
    public HotKeyBindingState State { get; } = state;
}