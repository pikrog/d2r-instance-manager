namespace AvaloniaApplication1.Engine.Models.Platform.Process;

public enum ProcessStartFailureReason
{
    FileNotFound,
    AccessDenied,
    InvalidExecutableFormat,
    DllNotFound,
    Unknown
}