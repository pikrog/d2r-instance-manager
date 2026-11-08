using System;

namespace D2RInstanceManager.HotKey.Platform.Registration.Native;

public sealed class HotKeyNativeRegistrationStateChangedEventArgs(HotKeyRegistrationToken token, HotKeyNativeRegistrationState state) : EventArgs
{
    public HotKeyRegistrationToken Token { get; } = token;
    
    public HotKeyNativeRegistrationState State { get; } = state;
}