using System;

namespace AvaloniaApplication1.HotKey;

public class HotKeyPressedEventArgs(KeyCombination keyCombination) : EventArgs
{
    public KeyCombination KeyCombination { get; } = keyCombination;
}