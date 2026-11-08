namespace D2RInstanceManager.HotKey.Coordination;

public enum HotKeyBindingState
{
    NotConfigured,
    Pending,
    Registered,
    ExternalConflict,
    InternalConflict,
    UnknownError,
}