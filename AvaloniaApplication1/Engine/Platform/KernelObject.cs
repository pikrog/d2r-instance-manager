using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using System.Threading;
using AvaloniaApplication1.Engine.Exceptions.Platform;
using AvaloniaApplication1.Engine.Models.Platform.Process;

namespace AvaloniaApplication1.Engine.Platform;

public class KernelObject(ProcessIdentity processIdentity, IntPtr sourceHandle, string type, string name)
{
    public ProcessIdentity ProcessIdentity { get; } = processIdentity;

    public IntPtr SourceHandle { get; } = sourceHandle;

    public string Name { get; } = name;

    public string Type { get; } = type;

    public enum CloseResult
    {
        Success,
        Failure,
    }
    
    public CloseResult CloseSource()
    {
        var processResult = Process.GetProcessById(ProcessIdentity.Id);
        if (!processResult.IsSuccess)
            return CloseResult.Failure;
        using var process = processResult.Value;
        if (process.Identity != ProcessIdentity)
            return CloseResult.Failure; // CloseResult.DifferentProcess
        // todo:
        // suspend process?
        // duplicate handle without inheriting
        // check handle identity
        var transferredHandle =
            DuplicateHandle(process.Handle, SourceHandle, options: WinApi.DuplicateOptions.CloseSource);
        if (transferredHandle is null)
            return CloseResult.Failure;
        transferredHandle.Dispose();
        // todo:
        // dispose test handle
        // resume process?
        return CloseResult.Success;
    }
    
    public static IEnumerable<KernelObject> GetAll(Func<string, bool> typeFilter, Func<string, bool>? nameFilter = null, CancellationToken cancellationToken = default)
    {
        Dictionary<uint, Process?> processes = [];
        try
        {
            var allHandles = GetAllSystemHandles(cancellationToken);
            foreach (var handle in allHandles)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var processId = checked((uint)handle.UniqueProcessId.ToInt64());
                if (!processes.TryGetValue(processId, out var process))
                {
                    process = Process.GetProcessById(processId).Value;
                    processes[processId] = process;
                }

                if (process is null)
                    continue;

                IntPtr processHandle;
                try
                {
                    processHandle = process.Handle;
                }
                catch (Win32Exception)
                {
                    processes[processId] = null;
                    continue;
                }

                using var duplicatedHandle = DuplicateHandle(processHandle, handle.HandleValue);
                if (duplicatedHandle is null)
                    continue;
                var type = GetObjectTypeName(duplicatedHandle.DangerousGetHandle()) ?? string.Empty;
                if (!typeFilter(type))
                    continue;
                var name = GetObjectFileName(duplicatedHandle.DangerousGetHandle()) ?? string.Empty;
                if (nameFilter is not null && !nameFilter(name))
                    continue;
                yield return new KernelObject(process.Identity, handle.HandleValue, type, name);
            }
        }
        finally
        {
            foreach (var (_, process) in processes)
                process?.Dispose();
        }
    }
    
    #region WinApi Helpers
    private const uint InitialSystemInfoListSize = 1024;
    
    private static List<WinApi.SystemHandleEx> GetAllSystemHandles(CancellationToken cancellationToken = default)
    {
        var handleInformationSize = InitialSystemInfoListSize;
        var handleList = new List<WinApi.SystemHandleEx>();
        var handleInformationPtr = Marshal.AllocHGlobal(checked((int)handleInformationSize));
        try
        {
            while (true)
            {
                cancellationToken.ThrowIfCancellationRequested();
                
                var status = WinApi.NtQuerySystemInformation(WinApi.SystemInformationClass.SystemExtendedHandleInformation, handleInformationPtr, handleInformationSize, out handleInformationSize);
                if (status == WinApi.NtStatus.Success)
                    break;
                if (status != WinApi.NtStatus.InfoLengthMismatch)
                    throw new NtException(status);

                Marshal.FreeHGlobal(handleInformationPtr);
                handleInformationPtr = IntPtr.Zero;
                handleInformationSize *= 2;
                handleInformationPtr = Marshal.AllocHGlobal(checked((int)(handleInformationSize)));
            }
            var handleInformation = Marshal.PtrToStructure<WinApi.SystemHandleInformationEx>(handleInformationPtr);
            var handlesOffset = Marshal.SizeOf<WinApi.SystemHandleInformationEx>();
            var handlesPtr = handleInformationPtr + handlesOffset;
            for (uint i = 0; i < handleInformation.HandleCount; i++)
            {
                var handle = Marshal.PtrToStructure<WinApi.SystemHandleEx>(handlesPtr);
                handlesPtr += Marshal.SizeOf<WinApi.SystemHandleEx>();
                handleList.Add(handle);
            }
        }
        finally
        {
            if (handleInformationPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(handleInformationPtr);
        }

        return handleList;
    }
    
    private const uint InitialObjectInformationSize = 1024;
    
    private static string? GetObjectTypeName(IntPtr handle)
    {
        return GetObjectUnicodeString(handle, WinApi.ObjectInformationClass.ObjectTypeInformation);
    }
    
    private static string? GetObjectFileName(IntPtr handle)
    {
        return GetObjectUnicodeString(handle, WinApi.ObjectInformationClass.ObjectNameInformation);
    }
    
    private static string? GetObjectUnicodeString(IntPtr handle, WinApi.ObjectInformationClass informationClass)
    {
        var objectInformationSize = InitialObjectInformationSize;
        var objectInformationPtr = Marshal.AllocHGlobal(checked((int)objectInformationSize));
        try
        {
            var status = WinApi.NtQueryObject(handle, informationClass, objectInformationPtr, objectInformationSize, out var returnLength);
            if (status != WinApi.NtStatus.Success)
            {
                if (status != WinApi.NtStatus.InfoLengthMismatch)
                    return null;

                // buffer too small, resize and try again
                Marshal.FreeHGlobal(objectInformationPtr);
                objectInformationPtr = IntPtr.Zero;
                objectInformationSize = returnLength;
                objectInformationPtr = Marshal.AllocHGlobal(checked((int)objectInformationSize));
                
                status = WinApi.NtQueryObject(handle, informationClass, objectInformationPtr, objectInformationSize, out _);
                if (status != WinApi.NtStatus.Success)
                    return null;
            }

            var name = Marshal.PtrToStructure<WinApi.UnicodeString>(objectInformationPtr);
            return name.ToString();
        }
        finally
        {
            if (objectInformationPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(objectInformationPtr);
        }
    }
    
    private static SafeKernelObjectHandle? DuplicateHandle(IntPtr sourceProcessHandle, IntPtr sourceHandle, WinApi.AccessMask desiredAccess = 0, uint attributes = 0, WinApi.DuplicateOptions options = WinApi.DuplicateOptions.Default)
    {
        var process = Process.GetCurrentProcess();
        if (!process.IsSuccess)
            return null;
        var status = WinApi.NtDuplicateObject(sourceProcessHandle, sourceHandle, process.Value.Handle, out var duplicatedHandle, desiredAccess, attributes, options);
        return status == WinApi.NtStatus.Success 
            ? new SafeKernelObjectHandle(duplicatedHandle, true)
            : null;
    }
    #endregion
}
