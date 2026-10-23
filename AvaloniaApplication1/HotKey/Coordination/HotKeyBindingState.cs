namespace AvaloniaApplication1.HotKey.Coordination;

public enum HotKeyBindingState
{
    NotConfigured,
    Pending,
    Registered,
    ExternalConflict,
    InternalConflict,
    UnknownError,
}