using System;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Platform.Process;

public record ProcessError(int NativeErrorCode, ProcessOperation Operation, uint? Id = null, IntPtr? Handle = null)
{
    public ProcessFailureReason FailureReason
    {
        get
        {
            var error = (WinApi.Win32Error) NativeErrorCode;
            return Operation switch
            {
                ProcessOperation.Open => error switch
                {
                    WinApi.Win32Error.AccessDenied => ProcessFailureReason.AccessDenied,
                    WinApi.Win32Error.InvalidParameter => ProcessFailureReason.ProcessNotFound,
                    _ => ProcessFailureReason.Unknown
                },
                ProcessOperation.Query => error switch
                {
                    WinApi.Win32Error.AccessDenied => ProcessFailureReason.AccessDenied,
                    WinApi.Win32Error.InvalidHandle => ProcessFailureReason.InvalidHandle,
                    _ => ProcessFailureReason.Unknown
                },
                _ => throw new InvalidOperationException($"Unknown operation: {Operation}")
            };
        }
    }
}