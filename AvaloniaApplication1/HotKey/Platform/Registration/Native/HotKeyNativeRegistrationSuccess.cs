namespace AvaloniaApplication1.HotKey.Platform.Registration.Native;

public sealed record HotKeyNativeRegistrationSuccess : HotKeyNativeRegistrationState
{
    public static readonly HotKeyNativeRegistrationSuccess Instance = new();
}