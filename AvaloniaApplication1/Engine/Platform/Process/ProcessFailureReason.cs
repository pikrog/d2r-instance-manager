namespace AvaloniaApplication1.Engine.Platform.Process;

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