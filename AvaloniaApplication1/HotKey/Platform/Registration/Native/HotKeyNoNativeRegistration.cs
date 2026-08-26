namespace AvaloniaApplication1.HotKey.Platform.Registration.Native;

public sealed record HotKeyNoNativeRegistration : HotKeyNativeRegistrationState
{
    public static readonly HotKeyNoNativeRegistration Instance = new();
};