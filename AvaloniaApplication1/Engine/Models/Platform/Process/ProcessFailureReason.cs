namespace AvaloniaApplication1.Engine.Models.Platform.Process;

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