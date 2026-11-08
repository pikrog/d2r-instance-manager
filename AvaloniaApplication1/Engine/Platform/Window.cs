using System;

namespace D2RInstanceManager.Engine.Platform;

public static class Window
{
    public static RequestState BringToTop(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero)
            return RequestState.Rejected;

        WinApi.ShowWindow(windowHandle, WinApi.ShowCommand.Restore);
        var currentThreadId = WinApi.GetCurrentThreadId();
        var foregroundWindowHandle = WinApi.GetForegroundWindow();
        var foregroundWindowThreadId = WinApi.GetWindowThreadProcessId(foregroundWindowHandle, out _);
        if (foregroundWindowThreadId == 0 || currentThreadId != foregroundWindowThreadId
                                          && !WinApi.AttachThreadInput(foregroundWindowThreadId, currentThreadId, true)
                                          || !WinApi.BringWindowToTop(windowHandle))
            return RequestState.Rejected;
        WinApi.ShowWindow(windowHandle, WinApi.ShowCommand.Show);
        if (currentThreadId != foregroundWindowThreadId)
            WinApi.AttachThreadInput(foregroundWindowThreadId, currentThreadId, false);
        return RequestState.Accepted;
    }

    public static RequestState CenterMouseCursorInside(IntPtr windowHandle)
    {
        if (windowHandle == IntPtr.Zero || !WinApi.GetWindowRect(windowHandle, out var rect))
            return RequestState.Rejected;

        var x = (rect.Left + rect.Right) / 2;
        var y = (rect.Top + rect.Bottom) / 2;

        return WinApi.SetCursorPos(x, y) 
            ? RequestState.Accepted 
            : RequestState.Rejected;
    }
}