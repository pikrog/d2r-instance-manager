using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Platform.Process;

public record ProcessStartError(int NativeErrorCode, string FileName)
{
    public ProcessStartFailureReason FailureReason =>
        (WinApi.Win32Error) NativeErrorCode switch
        {
            WinApi.Win32Error.FileNotFound or WinApi.Win32Error.PathNotFound => ProcessStartFailureReason.FileNotFound,
            WinApi.Win32Error.AccessDenied => ProcessStartFailureReason.AccessDenied,
            WinApi.Win32Error.BadExeFormat => ProcessStartFailureReason.InvalidExecutableFormat,
            WinApi.Win32Error.DllNotFound => ProcessStartFailureReason.DllNotFound,
            _ => ProcessStartFailureReason.Unknown
        };
}