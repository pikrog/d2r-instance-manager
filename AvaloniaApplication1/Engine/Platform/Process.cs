using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Engine.Lang;
using AvaloniaApplication1.Engine.Models.Platform.Process;
using AvaloniaApplication1.Engine.Models.Platform.Results;
using Microsoft.Win32.SafeHandles;

namespace AvaloniaApplication1.Engine.Platform;

using HasExitedResult = Result<bool, ProcessError>;
using ExitCodeResult = Result<uint?, ProcessError>;
using KillResult = Result<RequestState, ProcessError>;
using ProcessIdResult = Result<uint, ProcessError>;
using CreationTimeResult = Result<long, ProcessError>;
using IdentityResult = Result<ProcessIdentity, ProcessError>;
using ProcessResult = Result<Process, ProcessError>;

public sealed class Process : IDisposable
{
    public IntPtr Handle => SafeHandle.DangerousGetHandle();
    
    public SafeProcessHandle SafeHandle { get; }

    private readonly Lazy<IntPtr> _mainWindowHandle;
    
    public IntPtr MainWindowHandle => _mainWindowHandle.Value;
    
    public ProcessIdentity Identity { get; }
    
    public uint Id => Identity.Id;
    
    public HasExitedResult CheckIfExited()
    {
        var result = (WinApi.WaitResult)WinApi.WaitForSingleObject(Handle, 0);
        return result switch
        {
            WinApi.WaitResult.Object0 => HasExitedResult.Success(true),
            WinApi.WaitResult.Timeout => HasExitedResult.Success(false),
            WinApi.WaitResult.Failed => HasExitedResult.Failure(ProcessError.ForQuery(Handle, Id)),
            _ => throw new InvalidOperationException($"Unexpected WaitForSingleObject result: {result}")
        };
    }

    public ExitCodeResult ExitCode
    {
        get
        {
            var hasExitedCheck = CheckIfExited();
            if (!hasExitedCheck.IsSuccess)
                return ExitCodeResult.Failure(hasExitedCheck.Error);
            var hasExited = hasExitedCheck.Value;
            if (!hasExited)
                return ExitCodeResult.Success(null);
            return WinApi.GetExitCodeProcess(Handle, out var exitCode) 
                ? ExitCodeResult.Success(exitCode)
                : ExitCodeResult.Failure(ProcessError.ForQuery(Handle, Id));
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
            if (processId != Id || !WinApi.IsWindowVisible(h)) 
                return true;
            handle = h;
            return false;
        }, IntPtr.Zero);
        
        return handle;
    }
    
    public RequestState CloseMainWindow() // todo: RequestState enum { Requested, NotRequested }
    {
        var mainWindowHandle = MainWindowHandle;
        return mainWindowHandle != IntPtr.Zero 
               && WinApi.PostMessage(mainWindowHandle, WinApi.WindowMessages.Close, IntPtr.Zero, IntPtr.Zero)
               ? RequestState.Accepted
               : RequestState.Rejected;
    }

    public void Suspend()
    {
        throw new NotImplementedException();
    }
    
    public KillResult Kill(uint exitCode = 0)
    {
        if (WinApi.TerminateProcess(Handle, exitCode))
            return KillResult.Success(RequestState.Accepted);
        var lastError = Marshal.GetLastPInvokeError();
        var hasExitedCheck = CheckIfExited();
        if (!hasExitedCheck.IsSuccess)
            return KillResult.Failure(hasExitedCheck.Error);
        var hasExited = hasExitedCheck.Value;
        return hasExited
            ? KillResult.Success(RequestState.Rejected) 
            : KillResult.Failure(ProcessError.ForKill(Handle, Id, lastError));
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

    private static ProcessIdResult GetProcessId(IntPtr handle, uint? expectedId = null)
    {
        var id = WinApi.GetProcessId(handle);
        if (id == 0) 
            return ProcessIdResult.Failure(ProcessError.ForQuery(handle, expectedId));
        if (expectedId is not null && id != expectedId)
            throw new ArgumentException($"Process ID mismatch: expected {expectedId}, got {id}");
        return ProcessIdResult.Success(id);

    }
    
    private static CreationTimeResult GetProcessCreationTime(IntPtr handle, uint? knownId = null)
    {
        return WinApi.GetProcessTimes(handle, out var creationTime, out _, out _, out _) 
            ? CreationTimeResult.Success(creationTime.ToLong()) 
            : CreationTimeResult.Failure(ProcessError.ForQuery(handle, knownId));
    }

    private static IdentityResult CreateIdentity(IntPtr handle, uint? expectedId = null)
    {
        var id = GetProcessId(handle, expectedId);
        if (!id.IsSuccess)
            return IdentityResult.Failure(id.Error);
        
        var creationTime = GetProcessCreationTime(handle, id.Value);
        if (!creationTime.IsSuccess)
            return IdentityResult.Failure(creationTime.Error);
        
        return IdentityResult.Success(new ProcessIdentity(id.Value, creationTime.Value));
    }
    
    public static ProcessResult Start(ProcessStartInfo startInfo)
    {
        var mask = WinApi.ShellExecuteMask.NoCloseProcess;
        if (startInfo.DisplayHandle is not null)
            mask |= WinApi.ShellExecuteMask.Monitor;
        var monitorHandle = startInfo.DisplayHandle ?? IntPtr.Zero;
        var info = new WinApi.ShellExecuteInfo()
        {
            Size = Marshal.SizeOf<WinApi.ShellExecuteInfo>(),
            File = startInfo.FileName,
            Parameters = startInfo.Arguments,
            Show = WinApi.ShowCommand.ShowNormal,
            Mask = mask,
            MonitorHandle = monitorHandle,
            Verb = WinApi.ShellExecuteVerbs.Open,
        };
        return WinApi.ShellExecuteEx(ref info)
            ? GetProcessByOwnedHandle(info.ProcessHandle) 
            : ProcessResult.Failure(ProcessError.ForStart(startInfo.FileName));
    }

    public static ProcessResult GetProcessByOwnedHandle(IntPtr handle, uint? expectedProcessId = null) =>
        GetProcessByHandle(handle, true, expectedProcessId);
    
    public static ProcessResult GetProcessByBorrowedHandle(IntPtr handle, uint? expectedProcessId = null) =>
        GetProcessByHandle(handle, false, expectedProcessId);
    
    private static ProcessResult GetProcessByHandle(IntPtr handle, bool ownsHandle, uint? expectedProcessId = null)
    {
        var runtimeInfo = CreateIdentity(handle, expectedProcessId);
        if (!runtimeInfo.IsSuccess)
            return ProcessResult.Failure(runtimeInfo.Error);
        var safeHandle = new SafeProcessHandle(handle, ownsHandle);
        return ProcessResult.Success(new Process(safeHandle, runtimeInfo.Value));
    }

    public static ProcessResult GetProcessById(uint processId)
    {
        var handle = WinApi.OpenProcess(WinApi.ProcessAccessFlags.All, false, processId);
        return handle != IntPtr.Zero
            ? GetProcessByOwnedHandle(handle, processId)
            : ProcessResult.Failure(ProcessError.ForOpen(processId));
    }

    public static ProcessResult GetCurrentProcess()
    {
        var handle = WinApi.GetCurrentProcess();
        return GetProcessByBorrowedHandle(handle);
    }
}