using System;
using System.Runtime.InteropServices;

namespace AvaloniaApplication1.Engine.Platform;

internal static partial class WinApi
{
    internal enum NtStatus : uint
    {
        Success             = 0x00000000,
        InfoLengthMismatch  = 0xc0000004,
        NotSupported        = 0xc00000bb,
        InvalidHandle       = 0xc0000008,
        AccessDenied        = 0xc0000022,
    }

    internal enum SystemInformationClass
    {
        SystemExtendedHandleInformation = 0x0040,
    }

    internal enum ObjectInformationClass : uint
    {
        ObjectBasicInformation  = 0x0,
        ObjectNameInformation   = 0x1,
        ObjectTypeInformation   = 0x2,
    }

    [Flags]
    internal enum AccessMask : uint { }

    [Flags]
    internal enum DuplicateOptions : uint
    {
        Default     = 0x0,
        CloseSource = 0x1,
        SameAccess  = 0x2,
    }

    internal enum ShowCommand : int
    {
        Hide            = 0,
        ShowNormal      = 1,
        Normal          = ShowNormal,
        ShowMinimized   = 2,
        ShowMaximized   = 3,
        Maximize        = ShowMaximized,
        ShowNoActive    = 4,
        Show            = 5,
        Minimize        = 6,
        ShowMinNoActive = 7,
        ShowNa          = 8,
        Restore         = 9,
        ShowDefault     = 10,
        ForceMinimize   = 11,
        Max             = ForceMinimize
    }

    [Flags]
    internal enum ShellExecuteMask : uint
    {
        Default         = 0x00000000,
        Monitor         = 0x00200000,
        NoCloseProcess  = 0x00000040,
    }
    
    [Flags]
    internal enum ProcessAccessFlags : uint
    {
        All = 0x001F0FFF,
        Terminate = 0x00000001,
        CreateThread = 0x00000002,
        VirtualMemoryOperation = 0x00000008,
        VirtualMemoryRead = 0x00000010,
        VirtualMemoryWrite = 0x00000020,
        DuplicateHandle = 0x00000040,
        CreateProcess = 0x000000080,
        SetQuota = 0x00000100,
        SetInformation = 0x00000200,
        QueryInformation = 0x00000400,
        QueryLimitedInformation = 0x00001000,
        Synchronize = 0x00100000
    }

    internal enum Win32Error
    {
        Success = 0,
        FileNotFound = 0x2,
        PathNotFound = 0x3,
        AccessDenied = 0x5,
        InvalidHandle = 0x6,
        NotEnoughMemory = 0x8,
        InvalidParameter = 0x57,
        AlreadyExists = 0xb7,
        InvalidName = 0x123,
        BadExeFormat = 0xc1,
        DllNotFound = 0x485,
    }

    internal enum ExitCode : uint
    {
        StillActive = 0x103,
    }

    internal enum WaitResult : uint
    {
        Object0 = 0x0,
        Timeout = 0x102,
        Failed = 0xFFFFFFFF,
    }

    internal static class WindowMessages
    {
        public const uint Close = 0x0010;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemHandleEx
    {
        public IntPtr Object;
        public IntPtr UniqueProcessId;
        public IntPtr HandleValue;
        public uint GrantedAccess;
        public ushort CreatorBackTraceIndex;
        public ushort ObjectTypeIndex;
        public uint HandleAttribtues;
        public uint Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct SystemHandleInformationEx
    {
        public UIntPtr HandleCount;
        public UIntPtr Reserved;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct UnicodeString : IDisposable
    {
        public ushort Length;
        public ushort MaximumLength;
        private IntPtr _buffer;

        public UnicodeString(string s)
        {
            Length = (ushort)(s.Length * 2);
            MaximumLength = (ushort)(Length + 2);
            _buffer = Marshal.StringToHGlobalUni(s);
        }

        public void Dispose()
        {
            Marshal.FreeHGlobal(_buffer);
            _buffer = IntPtr.Zero;
        }

        public readonly override string? ToString()
        {
            return _buffer == IntPtr.Zero ? null : Marshal.PtrToStringUni(_buffer, Length / 2);
        }
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct Rect
    {
        public int Left;
        public int Top;
        public int Right;
        public int Bottom;
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct ShellExecuteInfo
    {
        public int Size;
        public ShellExecuteMask Mask;
        public IntPtr WindowHandle;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Verb;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string File;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Parameters;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Directory;
        public ShowCommand Show;
        public IntPtr InstApp;
        public IntPtr IdList;
        [MarshalAs(UnmanagedType.LPTStr)]
        public string Class;
        public IntPtr ClassKey;
        public uint HotKey;
        public IntPtr MonitorHandle;
        public IntPtr ProcessHandle;
    }
    
    internal static class ShellExecuteVerbs
    {
        public const string Open = "open";
        //public const string RunAs = "runas";
    }

    [StructLayout(LayoutKind.Sequential)]
    internal struct FileTime
    {
        public uint LowDateTime;
        public uint HighDateTime;
        
        public long ToLong() => (long)HighDateTime << 32 | LowDateTime;
        
        public DateTime ToDateTime() => DateTime.FromFileTime(ToLong());
    }

    [Flags]
    internal enum HotKeyModifiers : uint
    {
        None        = 0x0000,
        Alt         = 0x0001,
        Control     = 0x0002,
        NoRepeat    = 0x4000,
        Shift       = 0x0004,
        Win         = 0x0008,
    }
    
    internal static WinApi.Win32Error GetLastPInvokeError() => (Win32Error)Marshal.GetLastPInvokeError();

    [LibraryImport("ntdll.dll")]
    internal static partial NtStatus NtQuerySystemInformation(SystemInformationClass systemInformationClass, IntPtr systemInformation, uint systemInformationLength, out uint returnLength);

    [LibraryImport("ntdll.dll")]
    internal static partial NtStatus NtDuplicateObject(IntPtr sourceProcessHandle, IntPtr sourceHandle, IntPtr targetProcessHandle, out IntPtr targetHandle, AccessMask desiredAccess, uint attributes, DuplicateOptions options);

    [LibraryImport("ntdll.dll")]
    internal static partial NtStatus NtQueryObject(IntPtr objectHandle, ObjectInformationClass informationClass, IntPtr informationPtr, uint informationLength, out uint returnLength);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool CloseHandle(IntPtr objectHandle);

    [DllImport("shell32.dll", CharSet = CharSet.Auto, SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static extern bool ShellExecuteEx(ref ShellExecuteInfo shellExecuteInfo); // todo: use LibraryImport?

    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate bool EnumMonitorsDelegate(IntPtr monitorHandle, IntPtr monitorDeviceContextHandle, ref Rect monitorRect, IntPtr data);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnumDisplayMonitors(IntPtr deviceContextHandle, IntPtr clip, EnumMonitorsDelegate enumFunction, IntPtr data);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool ShowWindow(IntPtr windowHandle, ShowCommand showCommand);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool RegisterHotKey(IntPtr windowHandle, int hotKeyId, HotKeyModifiers modifiers, uint virtualKeyCode);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool UnregisterHotKey(IntPtr windowHandle, int hotKeyId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool AttachThreadInput(uint attachedThreadId, uint attachedToThreadId, [MarshalAs(UnmanagedType.Bool)] bool doAttach);

    [LibraryImport("user32.dll")]
    internal static partial IntPtr GetForegroundWindow();

    [LibraryImport("kernel32.dll")]
    internal static partial uint GetCurrentThreadId();

    [LibraryImport("user32.dll", SetLastError = true)]
    internal static partial uint GetWindowThreadProcessId(IntPtr windowHandle, out uint processId);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool BringWindowToTop(IntPtr windowHandle);

    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetWindowRect(IntPtr windowHandle, out Rect rect);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial IntPtr OpenProcess(ProcessAccessFlags desiredAccess, [MarshalAs(UnmanagedType.Bool)] bool inheritHandle, uint processId);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetProcessTimes(IntPtr processHandle, out FileTime creationTime, out FileTime exitTime, out FileTime kernelTime, out FileTime userTime);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial uint GetProcessId(IntPtr handle);

    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial IntPtr GetCurrentProcess();
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool TerminateProcess(IntPtr processHandle, uint exitCode);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool GetExitCodeProcess(IntPtr processHandle, out uint exitCode);
    
    [LibraryImport("kernel32.dll", SetLastError = true)]
    internal static partial uint WaitForSingleObject(IntPtr handle, uint milliseconds);
    
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool PostMessage(IntPtr windowHandle, uint message, IntPtr wParam, IntPtr lParam);
    
    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool EnumWindows(EnumWindowsDelegate enumFunction, IntPtr lParam);
    
    [UnmanagedFunctionPointer(CallingConvention.StdCall)]
    internal delegate bool EnumWindowsDelegate(IntPtr windowHandle, IntPtr lParam);

    [LibraryImport("user32.dll")]
    [return: MarshalAs(UnmanagedType.Bool)]
    internal static partial bool IsWindowVisible(IntPtr windowHandle);
}