using System;

namespace AvaloniaApplication1.Engine.Exceptions.Platform;

public class ProcessStartException : ProcessException
{
    public enum ProcessStartFailureReason
    {
        FileNotFound,
        AccessDenied,
        InvalidExecutableFormat,
        DllNotFound,
    }

    private static string MapFailureReasonToMessage(ProcessStartFailureReason reason)
    {
        return reason switch
        {
            ProcessStartFailureReason.AccessDenied => "Access denied to file",
            ProcessStartFailureReason.FileNotFound => "File not found",
            ProcessStartFailureReason.InvalidExecutableFormat => "Executable file has invalid format",
            ProcessStartFailureReason.DllNotFound => "Executable is missing required DLL dependencies",
            _ => throw new InvalidOperationException($"Unknown failure reason: {reason}")
        };
    }

    public ProcessStartException(ProcessStartFailureReason failureReason) : base(MapFailureReasonToMessage(failureReason))
    {

    }

    public ProcessStartException(string message, Exception inner) : base(message, inner)
    {
    }
}