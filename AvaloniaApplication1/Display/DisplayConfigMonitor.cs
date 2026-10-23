using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Controls;
using AvaloniaApplication1.Engine.Platform;
using Window = Avalonia.Controls.Window;

namespace AvaloniaApplication1.Display;

public class DisplayConfigMonitor : IDisplayConfigMonitor, IDisplayConfigMonitorWindowInitializer
{
    public event EventHandler? DisplayConfigChanged;

    private const int WmDisplayChange = 0x007e;

    private const int WmSettingChange = 0x001a;

    private const int WmDeviceChange = 0x0219;

    private IReadOnlySet<DisplayInfo> _displays = new HashSet<DisplayInfo>(); 

    private IntPtr WndProc(IntPtr windowHandle, uint message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message is not (WmDisplayChange or WmSettingChange or WmDeviceChange)) 
            return IntPtr.Zero;
        
        var displays = DisplayInfo.GetAll().ToHashSet();
        if (!displays.SetEquals(_displays))
            DisplayConfigChanged?.Invoke(this, EventArgs.Empty);

        _displays = displays;

        return IntPtr.Zero;
    }

    public void Initialize(Window window)
    {
        Win32Properties.AddWndProcHookCallback(window, WndProc);

        _displays = DisplayInfo.GetAll().ToHashSet();
    }
}