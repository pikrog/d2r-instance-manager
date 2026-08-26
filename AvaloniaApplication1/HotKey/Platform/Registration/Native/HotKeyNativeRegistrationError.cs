using System;
using System.ComponentModel;

namespace AvaloniaApplication1.HotKey.Platform.Registration.Native;

public sealed class HotKeyNativeRegistrationError(int errorCode)
{
    private const int AlreadyRegisteredErrorCode = 0x0581;

    public bool IsAlreadyRegistered { get; } = errorCode == AlreadyRegisteredErrorCode;

    public Exception Exception { get; } = new Win32Exception(errorCode);
};