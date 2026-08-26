using System;

namespace AvaloniaApplication1.HotKey.Coordination;

public class HotKeyBindingStateChangedEventArgs<THotKeyCommand>(THotKeyCommand command, HotKeyBindingState state) 
    : EventArgs
    where THotKeyCommand : HotKeyCommand
{
    public THotKeyCommand Command { get; } = command;
        
    public HotKeyBindingState State { get; } = state;
}