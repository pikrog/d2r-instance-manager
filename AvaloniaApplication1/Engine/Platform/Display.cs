using System;
using System.Collections.Generic;

namespace AvaloniaApplication1.Engine.Platform;

public class Display(int index, IntPtr handle)
{
    public int Index { get; } = index;
    
    public IntPtr Handle { get; } = handle;

    public static Display GetPrimary => GetByIndex(0);

    public static Display GetByIndex(int index)
    {
        var list = GetAll();
        return list[index];
    }
    
    public static IReadOnlyList<Display> GetAll()
    {
        int index = 0;
        var list = new List<Display>();
        WinApi.EnumDisplayMonitors(
            IntPtr.Zero,
            IntPtr.Zero,
            (IntPtr monitorHandle, IntPtr monitorDisplayContextHandle, ref WinApi.Rect monitorRect, IntPtr data) =>
            {
                var display = new Display(index++, monitorHandle);
                list.Add(display);
                return true;
            },
            IntPtr.Zero
        );
        return list;
    }
}