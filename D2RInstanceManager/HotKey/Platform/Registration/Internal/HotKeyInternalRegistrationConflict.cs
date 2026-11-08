namespace D2RInstanceManager.HotKey.Platform.Registration.Internal;

public sealed record HotKeyInternalRegistrationConflict : HotKeyInternalRegistrationResult
{
    public static readonly HotKeyInternalRegistrationConflict Instance = new();
};