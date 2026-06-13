using System;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartException(ProcessStartFailureReason failureReason, string fileName, int nativeErrorCode)
    : ProcessException(MapFailureReasonToMessage(failureReason, fileName, nativeErrorCode))
{
    public ProcessStartFailureReason FailureReason { get; } = failureReason;

    public string FileName { get; } = fileName;

    public int NativeErrorCode { get; } = nativeErrorCode;

    private static string MapFailureReasonToMessage(ProcessStartFailureReason reason, string fileName, int nativeErrorCode)
    {
        return reason switch
        {
            ProcessStartFailureReason.AccessDenied => $"Access denied to file {fileName}",
            ProcessStartFailureReason.FileNotFound => "File not found",
            ProcessStartFailureReason.InvalidExecutableFormat => "Executable file has invalid format",
            ProcessStartFailureReason.DllNotFound => "Executable is missing required DLL dependencies",
            ProcessStartFailureReason.Unknown => $"Unknown error 0x{nativeErrorCode:x8}",
            _ => throw new InvalidOperationException($"Unknown failure reason: {reason}")
        };
    }
}