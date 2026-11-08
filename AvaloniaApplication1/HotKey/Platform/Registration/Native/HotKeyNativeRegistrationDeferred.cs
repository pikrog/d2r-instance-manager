namespace D2RInstanceManager.HotKey.Platform.Registration.Native;

public sealed record HotKeyNativeRegistrationDeferred : HotKeyNativeRegistrationState
{
    public static readonly HotKeyNativeRegistrationDeferred Instance = new();
}