using AvaloniaApplication1.HotKey.Platform.Registration.Native;

namespace AvaloniaApplication1.HotKey.Platform.Registration.Internal;

public sealed record HotKeyInternalRegistrationSuccess(
    HotKeyHandle Handle,
    HotKeyRegistrationToken Token,
    HotKeyNativeRegistrationState State
    ) : HotKeyInternalRegistrationResult;