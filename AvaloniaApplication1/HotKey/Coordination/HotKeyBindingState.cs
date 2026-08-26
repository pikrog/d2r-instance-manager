namespace AvaloniaApplication1.HotKey.Coordination;

public enum HotKeyBindingState
{
    NotConfigured,
    Registered,
    ExternalConflict,
    InternalConflict,
    UnknownError,
}