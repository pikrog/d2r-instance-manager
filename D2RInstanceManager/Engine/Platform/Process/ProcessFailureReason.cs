namespace D2RInstanceManager.Engine.Platform.Process;

public enum ProcessFailureReason
{
    InvalidHandle,
    AccessDenied,
    ProcessNotFound,
    
    FileNotFound,
    InvalidExecutableFormat,
    DllNotFound,
    
    Unknown,
}