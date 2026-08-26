using System;
using AvaloniaApplication1.Common;
using AvaloniaApplication1.HotKey.Platform.Registration.Native;

namespace AvaloniaApplication1.HotKey.Platform;

public interface IHotKeyService
{
    Result<Unit, HotKeyNativeRegistrationError> Register(KeyCombination keyCombination);
    void Unregister(KeyCombination keyCombination);
    event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;
}