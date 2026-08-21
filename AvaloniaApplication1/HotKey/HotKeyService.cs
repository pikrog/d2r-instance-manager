using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Runtime.InteropServices;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Win32.Input;

namespace AvaloniaApplication1.HotKey;

public partial class HotKeyService : IHotKeyService, IHotKeyWindowInitializer
{
    [Flags]
    private enum PlatformKeyModifiers : uint
    {
        None        = 0x0000,
        Alt         = 0x0001,
        Control     = 0x0002,
        NoRepeat    = 0x4000,
        Shift       = 0x0004,
        Win         = 0x0008,
    }

    private record PlatformHotKey(PlatformKeyModifiers Modifiers, uint VirtualKeyCode)
    {
        private static PlatformKeyModifiers ToPlatformModifiers(KeyModifiers modifiers)
        {
            var result = PlatformKeyModifiers.None;
            if (modifiers.HasFlag(KeyModifiers.Alt))
                result |= PlatformKeyModifiers.Alt;
            if (modifiers.HasFlag(KeyModifiers.Control))
                result |= PlatformKeyModifiers.Control;
            if (modifiers.HasFlag(KeyModifiers.Shift))
                result |= PlatformKeyModifiers.Shift;
            if (modifiers.HasFlag(KeyModifiers.Meta))
                result |= PlatformKeyModifiers.Win;
            return result;
        }
        
        public static PlatformHotKey Create(KeyCombination keyCombination)
        {
            var modifiers = ToPlatformModifiers(keyCombination.KeyModifiers);
            var code = (uint)KeyInterop.VirtualKeyFromKey(keyCombination.Key);
            return new PlatformHotKey(modifiers, code);
        }
    }
    
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool RegisterHotKey(IntPtr windowHandle, int hotKeyId, PlatformKeyModifiers modifiers, uint virtualKeyCode);
    
    [LibraryImport("user32.dll", SetLastError = true)]
    [return: MarshalAs(UnmanagedType.Bool)]
    private static partial bool UnregisterHotKey(IntPtr windowHandle, int hotKeyId);
    
    private const uint WmHotKey = 0x0312;

    private const uint HotKeyAlreadyRegistered = 0x0581;

    private IntPtr WndProc(IntPtr windowHandle, uint message, IntPtr wParam, IntPtr lParam, ref bool handled)
    {
        if (message != WmHotKey)
            return IntPtr.Zero;
        
        var hotKeyId = wParam.ToInt32();
        
        if (!_hotKeyById.TryGetValue(hotKeyId, out var hotKey))
            return IntPtr.Zero;

        var eventArgs = new HotKeyPressedEventArgs(hotKey);
        HotKeyPressed?.Invoke(this, eventArgs);
        handled = true;

        return IntPtr.Zero;
    }
    
    private IntPtr _windowHandle;
    
    private readonly Dictionary<KeyCombination, int> _idByHotKey = new();

    private readonly Dictionary<int, KeyCombination> _hotKeyById = new();

    private const int MinHotKeyId = 0x0001;
    
    private const int MaxHotKeyId = 0xbfff;
    
    private int _nextHotKeyId = MinHotKeyId;

    private void IncrementHotKeyId()
    {
        _nextHotKeyId++;
        if (_nextHotKeyId > MaxHotKeyId)
            _nextHotKeyId = MinHotKeyId;
    }

    private void EnsureWindowInitialized()
    {
        if (_windowHandle == IntPtr.Zero)
            throw new InvalidOperationException("HotKeyService must be initialized with a window before registering hotkeys.");
    }

    public void Initialize(Window window)
    {
        var handle = window.TryGetPlatformHandle() ?? throw new InvalidOperationException("Window handle could not be retrieved.");
        _windowHandle = handle.Handle;
        Win32Properties.AddWndProcHookCallback(window, WndProc);
    }

    public bool Register(KeyCombination keyCombination)
    {
        EnsureWindowInitialized();
        
        if (_idByHotKey.ContainsKey(keyCombination))
            return true;

        var id = _nextHotKeyId;
        var (modifiers, code) = PlatformHotKey.Create(keyCombination);
        
        if (!RegisterHotKey(_windowHandle, id, modifiers, code))
        {
            var errorCode = Marshal.GetLastWin32Error();
            return errorCode == HotKeyAlreadyRegistered 
                ? false 
                : throw new Win32Exception(errorCode);
        }
        
        _idByHotKey[keyCombination] = id;
        _hotKeyById[id] = keyCombination;
        
        IncrementHotKeyId();
        
        return true;
    }

    public void Unregister(KeyCombination keyCombination)
    {
        EnsureWindowInitialized();
        
        if (!_idByHotKey.TryGetValue(keyCombination, out var id))
            return;
        
        if (!UnregisterHotKey(_windowHandle, id))
            throw new Win32Exception();
        
        _idByHotKey.Remove(keyCombination);
        _hotKeyById.Remove(id);
    }
    
    public event EventHandler<HotKeyPressedEventArgs>? HotKeyPressed;
}
