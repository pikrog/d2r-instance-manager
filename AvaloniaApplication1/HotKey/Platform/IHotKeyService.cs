using System;
using D2RInstanceManager.Common;
using D2RInstanceManager.HotKey.Platform.Registration.Native;

namespace D2RInstanceManager.HotKey.Platform;

public interface IHotKeyService
{
    bool IsReady { get; }
    event EventHandler BecameReady;
    
    Result<Unit, HotKeyNativeRegistrationError> Register(KeyCombination keyCombination);
    void Unregister(KeyCombination keyCombination);
    event EventHandler<HotKeyPressedEventArgs> HotKeyPressed;
}