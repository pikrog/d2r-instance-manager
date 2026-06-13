using System;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Exceptions;
using AvaloniaApplication1.Engine.Exceptions.Platform;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using Microsoft.Win32.SafeHandles;
using FileNotFoundException = System.IO.FileNotFoundException;

namespace AvaloniaApplication1.Engine.Platform;


public sealed class Process : IDisposable
{
    public IntPtr Handle => SafeHandle.DangerousGetHandle();
    
    public SafeProcessHandle SafeHandle { get; }

    private readonly Lazy<IntPtr> _mainWindowHandle;
    
    public IntPtr MainWindowHandle => _mainWindowHandle.Value;
    
    public ProcessIdentity Identity { get; }
    
    public uint Id => Identity.Id;
    
    public bool HasExited
    {
        get
        {
            var result = (WinApi.WaitResult)WinApi.WaitForSingleObject(Handle, 0);
            return result switch
            {
                WinApi.WaitResult.Object0 => true,
                WinApi.WaitResult.Timeout => false,
                WinApi.WaitResult.Failed => throw CreateGenericProcessException(),
                _ => throw new InvalidOperationException($"Unexpected WaitForSingleObject result: {result}")
            };
        }
    }

    public uint ExitCode
    {
        get
        {
            if (!HasExited)
                throw new ProcessStillActiveException(Id);
            return WinApi.GetExitCodeProcess(Handle, out var exitCode) 
                ? exitCode 
                : throw CreateGenericProcessException();
        }
    }

    private int _disposed;

    private Process(SafeProcessHandle handle, ProcessIdentity identity)
    {
        _mainWindowHandle = new Lazy<IntPtr>(FindMainWindowHandle);
        
        SafeHandle = handle;
        Identity = identity;
    }

    public void Dispose()
    {
        if (Interlocked.Exchange(ref _disposed, 1) == 1)
            return;
        SafeHandle.Dispose();
    }

    private IntPtr FindMainWindowHandle()
    {
        var handle = IntPtr.Zero;
        
        WinApi.EnumWindows((h, _) =>
        {
            WinApi.GetWindowThreadProcessId(h, out var processId);
            if (processId != Identity.Id || !WinApi.IsWindowVisible(h)) 
                return true;
            handle = h;
            return false;
        }, IntPtr.Zero);
        
        return handle;
    }
    
    public bool CloseMainWindow()
    {
        if (HasExited)
            throw new ProcessAlreadyExitedException(Id);
        var mainWindowHandle = MainWindowHandle;
        return mainWindowHandle != IntPtr.Zero 
               && WinApi.PostMessage(mainWindowHandle, WinApi.WindowMessages.Close, IntPtr.Zero, IntPtr.Zero);
    }

    public void Suspend()
    {
        throw new NotImplementedException();
    }
    
    public void Kill(uint exitCode = 0)
    {
        if (WinApi.TerminateProcess(Handle, exitCode))
            return;
        var lastError = WinApi.GetLastPInvokeError();
        if (HasExited)
            throw new ProcessAlreadyExitedException(Id);
        throw CreateGenericProcessException(lastError);
    }

    public Task WaitForExitAsync(CancellationToken cancellationToken = default)
    {
        var completionSource = new TaskCompletionSource();
        var @event = new AutoResetEvent(false) { SafeWaitHandle = new SafeWaitHandle(Handle, false) };
        var waitHandle = ThreadPool.RegisterWaitForSingleObject(
            @event,
            (_, _) => completionSource.TrySetResult(),
            null,
            Timeout.Infinite,
            executeOnlyOnce: true
            );

        if (cancellationToken.CanBeCanceled)
        {
            cancellationToken.Register(() =>
            {
                waitHandle.Unregister(null);
                completionSource.TrySetCanceled();
            });
        }
        
        return completionSource.Task;
    }

    private static uint GetProcessId(IntPtr handle, uint? expectedId = null)
    {
        var id = WinApi.GetProcessId(handle);
        if (id != 0)
        {
            if (expectedId is not null && id != expectedId)
                throw new ArgumentException($"Process ID mismatch: expected {expectedId}, got {id}");
            return id;
        }

        if (expectedId is not null)
            throw CreateGenericProcessException(expectedId.Value);

        throw CreateGenericProcessException(handle);
    }
    
    private static long GetProcessCreationTime(IntPtr handle, uint? knownId = null)
    {
        if (WinApi.GetProcessTimes(handle, out var creationTime, out _, out _, out _))
            return creationTime.ToLong();
        
        if (knownId is not null)
            throw CreateGenericProcessException(knownId.Value);
        
        throw CreateGenericProcessException(handle);
    }

    private static ProcessIdentity CreateIdentity(IntPtr handle, uint? expectedId = null)
    {
        var id = GetProcessId(handle, expectedId);
        var creationTime = GetProcessCreationTime(handle, id);
        return new ProcessIdentity(id, creationTime);
    }
    
    public static Process Start(ProcessStartInfo startInfo)
    {
        var mask = WinApi.ShellExecuteMask.NoCloseProcess;
        if (startInfo.DisplayHandle is not null)
            mask |= WinApi.ShellExecuteMask.Monitor;
        var info = new WinApi.ShellExecuteInfo()
        {
            Size = Marshal.SizeOf<WinApi.ShellExecuteInfo>(),
            File = startInfo.ExecutablePath,
            Parameters = startInfo.Arguments,
            Show = WinApi.ShowCommand.ShowNormal,
            Mask = mask,
            MonitorHandle = startInfo.DisplayHandle ?? IntPtr.Zero,
            Verb = WinApi.ShellExecuteVerbs.Open,
        };
        return WinApi.ShellExecuteEx(ref info)
            ? GetProcessByOwnedHandle(info.ProcessHandle) 
            : throw CreateProcessExceptionForShellExecuteEx(startInfo.ExecutablePath);
    }

    public static Process GetProcessByOwnedHandle(IntPtr handle, uint? expectedProcessId = null) =>
        GetProcessByHandle(handle, true, expectedProcessId);
    
    public static Process GetProcessByBorrowedHandle(IntPtr handle, uint? expectedProcessId = null) =>
        GetProcessByHandle(handle, false, expectedProcessId);
    
    private static Process GetProcessByHandle(IntPtr handle, bool ownsHandle, uint? expectedProcessId = null)
    {
        var runtimeInfo = CreateIdentity(handle, expectedProcessId);
        var safeHandle = new SafeProcessHandle(handle, ownsHandle);
        return new Process(safeHandle, runtimeInfo);
    }

    public static Process GetProcessById(uint processId)
    {
        var handle = WinApi.OpenProcess(WinApi.ProcessAccessFlags.All, false, processId);
        return handle != IntPtr.Zero 
            ? GetProcessByOwnedHandle(handle, processId) 
            : throw CreateProcessExceptionForOpenProcess(processId);
    }

    public static Process GetCurrentProcess()
    {
        var handle = WinApi.GetCurrentProcess();
        return GetProcessByBorrowedHandle(handle);
    }
    
    #region Exception factories
    private static ProcessException CreateProcessExceptionForShellExecuteEx(string executablePath,
        WinApi.Win32Error? error = null)
    {
        error ??= WinApi.GetLastPInvokeError();
        var failureReason = error switch
        {
            WinApi.Win32Error.FileNotFound or WinApi.Win32Error.PathNotFound => ProcessStartFailureReason.FileNotFound,
            WinApi.Win32Error.AccessDenied => ProcessStartFailureReason.AccessDenied,
            WinApi.Win32Error.BadExeFormat => ProcessStartFailureReason.InvalidExecutableFormat,
            WinApi.Win32Error.DllNotFound => ProcessStartFailureReason.DllNotFound,
            _ => ProcessStartFailureReason.Unknown,
        };
        return new ProcessStartException(failureReason, executablePath, (int)error);
    }

    private static ProcessException CreateProcessExceptionForOpenProcess(uint processId,
        WinApi.Win32Error? error = null)
    {
        error ??= WinApi.GetLastPInvokeError();
        return error switch
        {
            WinApi.Win32Error.InvalidParameter => new ProcessNotFoundException(processId),
            WinApi.Win32Error.AccessDenied => new ProcessAccessDeniedException(processId),
            _ => CreateExceptionForWin32Error(error.Value)
        };   
    }
        
    
    private ProcessException CreateGenericProcessException(WinApi.Win32Error? error = null) => CreateGenericProcessException(Id, error);

    private static ProcessException CreateGenericProcessException(IntPtr handle, WinApi.Win32Error? error = null)
    {
        error ??= WinApi.GetLastPInvokeError();
        return error switch
        {
            WinApi.Win32Error.AccessDenied => new ProcessAccessDeniedException(handle),
            WinApi.Win32Error.InvalidHandle => new ProcessInvalidHandleException(handle),
            _ => CreateExceptionForWin32Error(error.Value)
        };
    }
    
    private static ProcessException CreateGenericProcessException(uint id, WinApi.Win32Error? error = null)
    {
        error ??= WinApi.GetLastPInvokeError();
        return error switch
        {
            WinApi.Win32Error.AccessDenied => new ProcessAccessDeniedException(id),
            WinApi.Win32Error.InvalidHandle => new ProcessInvalidHandleException(id),
            _ => CreateExceptionForWin32Error(error.Value)
        };
    }
    
    private static ProcessUnknownError CreateExceptionForWin32Error(WinApi.Win32Error error) => 
        new(new Win32Exception((int)error));
    #endregion
}