using D2RInstanceManager.HotKey.Platform.Registration.Native;

namespace D2RInstanceManager.HotKey.Platform.Registration.Internal;

public sealed record HotKeyInternalRegistrationSuccess(
    HotKeyHandle Handle,
    HotKeyRegistrationToken Token,
    HotKeyNativeRegistrationState State
    ) : HotKeyInternalRegistrationResult;