namespace D2RInstanceManager.HotKey.Platform.Registration.Native;

public sealed record HotKeyNativeRegistrationSuccess : HotKeyNativeRegistrationState
{
    public static readonly HotKeyNativeRegistrationSuccess Instance = new();
}