using System;

namespace AvaloniaApplication1.HotKey;

public interface IHotKeyService
{
    bool Register(KeyCombination keyCombination);
    void Unregister(KeyCombination keyCombination);
    event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;
}