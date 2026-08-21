using System;

namespace AvaloniaApplication1.HotKey.Registration;

public class HotKeyRegistrationChangedEventArgs(KeyCombination keyCombination, HotKeyRegistration registration) : EventArgs
{
    public KeyCombination KeyCombination { get; } = keyCombination;
    
    public HotKeyRegistration Registration { get; } = registration;
}