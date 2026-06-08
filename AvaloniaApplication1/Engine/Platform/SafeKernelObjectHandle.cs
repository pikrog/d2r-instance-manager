using System;
using Microsoft.Win32.SafeHandles;

namespace AvaloniaApplication1.Engine.Platform;

internal sealed class SafeKernelObjectHandle : SafeHandleZeroOrMinusOneIsInvalid
{
    private SafeKernelObjectHandle() : base(true) { }

    public SafeKernelObjectHandle(IntPtr handle, bool ownsHandle) : base(ownsHandle)
    {
        SetHandle(handle);
    }

    protected override bool ReleaseHandle()
    {
        return WinApi.CloseHandle(handle);
    }
}