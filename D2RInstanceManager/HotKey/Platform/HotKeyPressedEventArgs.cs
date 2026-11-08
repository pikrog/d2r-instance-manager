using System;

namespace D2RInstanceManager.HotKey.Platform;

public class HotKeyPressedEventArgs(KeyCombination keyCombination) : EventArgs
{
    public KeyCombination KeyCombination { get; } = keyCombination;
}