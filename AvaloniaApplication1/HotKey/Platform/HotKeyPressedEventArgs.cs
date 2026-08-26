using System;

namespace AvaloniaApplication1.HotKey.Platform;

public class HotKeyPressedEventArgs(KeyCombination keyCombination) : EventArgs
{
    public KeyCombination KeyCombination { get; } = keyCombination;
}