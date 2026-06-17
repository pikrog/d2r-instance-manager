using System;
using System.Runtime.InteropServices;
using AvaloniaApplication1.Engine.Platform;

namespace AvaloniaApplication1.Engine.Models.Platform.Process;

public record ProcessError(int NativeErrorCode, ProcessOperation Operation, uint? Id = null, IntPtr? Handle = null, string? FileName = null)
{
    public static ProcessError ForOpen(uint id, IntPtr? handle = null, int? nativeErrorCode = null) =>
        new(nativeErrorCode ?? Marshal.GetLastPInvokeError(), ProcessOperation.Open, id, handle);
    
    public static ProcessError ForQuery(IntPtr handle, uint? id = null, int? nativeErrorCode = null) =>
        new(nativeErrorCode ?? Marshal.GetLastPInvokeError(), ProcessOperation.Query, id, handle);
    
    public static ProcessError ForStart(string fileName, int? nativeErrorCode = null) =>
        new(nativeErrorCode ?? Marshal.GetLastPInvokeError(), ProcessOperation.Start, FileName: fileName);
    
    public static ProcessError ForKill(IntPtr handle, uint? id = null, int? nativeErrorCode = null) =>
        new(nativeErrorCode ?? Marshal.GetLastPInvokeError(), ProcessOperation.Kill, id, handle);
    
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
                ProcessOperation.Query or ProcessOperation.Kill => error switch
                {
                    WinApi.Win32Error.AccessDenied => ProcessFailureReason.AccessDenied,
                    WinApi.Win32Error.InvalidHandle => ProcessFailureReason.InvalidHandle,
                    _ => ProcessFailureReason.Unknown
                },
                ProcessOperation.Start => error switch
                {
                    WinApi.Win32Error.FileNotFound or WinApi.Win32Error.PathNotFound => ProcessFailureReason.FileNotFound,
                    WinApi.Win32Error.AccessDenied => ProcessFailureReason.AccessDenied,
                    WinApi.Win32Error.BadExeFormat => ProcessFailureReason.InvalidExecutableFormat,
                    WinApi.Win32Error.DllNotFound => ProcessFailureReason.DllNotFound,
                    _ => ProcessFailureReason.Unknown
                },
                _ => throw new InvalidOperationException($"Unknown operation: {Operation}")
            };
        }
    }
}