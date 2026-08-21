namespace AvaloniaApplication1.HotKey.Registration;

public sealed record HotKeyRegistrationNotConfigured : HotKeyRegistration
{
    public static readonly HotKeyRegistrationNotConfigured Instance = new();
}