namespace AvaloniaApplication1.HotKey.Platform.Registration.Internal;

public sealed record HotKeyInternalRegistrationConflict : HotKeyInternalRegistrationResult
{
    public static readonly HotKeyInternalRegistrationConflict Instance = new();
};