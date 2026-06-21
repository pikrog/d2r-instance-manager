using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.Engine.Platform;

public class Display(int index, IntPtr handle, string id, string description, int width, int height, bool isPrimary)
{
    public int Index { get; } = index;
    
    public IntPtr Handle { get; } = handle;

    public string Id { get; } = id;
    
    public string Description { get; } = description;

    public int Width { get; } = width;

    public int Height { get; } = height;
    
    public bool IsPrimary { get; } = isPrimary;

    public static Display GetPrimary() => GetAll().Single(d => d.IsPrimary);

    public static Display GetByIndex(int index) => GetAll()[index];
    
    public static Display GetById(string id) => GetAll().Single(d => d.Id == id); // todo: don't use these functions
    
    public static IReadOnlyList<Display> GetAll()
    {
        var index = 0;
        var handleList = new List<Display>();
        WinApi.EnumDisplayMonitors(
            IntPtr.Zero,
            IntPtr.Zero,
            (monitorHandle, _, ref _, _) =>
            {
                var monitorInfo = new WinApi.MonitorInfoEx
                {
                    Size = Marshal.SizeOf<WinApi.MonitorInfoEx>()
                };
                
                var displayDevice = new WinApi.DisplayDevice
                {
                    Size = Marshal.SizeOf<WinApi.DisplayDevice>()
                };
                
                if (!WinApi.GetMonitorInfo(monitorHandle, ref monitorInfo))
                    return true;

                if (!WinApi.EnumDisplayDevices(monitorInfo.Device, 0, ref displayDevice, 0))
                    return true;
                
                var width = monitorInfo.Monitor.Right - monitorInfo.Monitor.Left;
                var height = monitorInfo.Monitor.Bottom - monitorInfo.Monitor.Top;
                var isPrimary = (monitorInfo.Flags & WinApi.MonitorInfoExFlags.Primary) == WinApi.MonitorInfoExFlags.Primary;
                var display = new Display(index++, monitorHandle, displayDevice.DeviceId, displayDevice.DeviceString, width, height, isPrimary);
                handleList.Add(display);
                
                return true;
            },
            IntPtr.Zero
        );
        return handleList;
    }
}