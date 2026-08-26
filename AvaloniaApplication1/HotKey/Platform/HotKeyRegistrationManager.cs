using System;
using System.Collections.Generic;
using System.Linq;
using Avalonia.Input;
using AvaloniaApplication1.HotKey.Platform.Registration.Internal;
using AvaloniaApplication1.HotKey.Platform.Registration.Native;

namespace AvaloniaApplication1.HotKey.Platform;

public class HotKeyRegistrationManager : IHotKeySuspensionCoordinator
{
    private sealed record HotKeyEntry(HotKeyHandle Handle, HotKeyRegistrationToken Token, HotKeyNativeRegistrationState State, Action Action);
    
    private readonly IHotKeyService _hotKeyService;

    private readonly Dictionary<KeyCombination, HotKeyEntry> _entries = new();
    
    private readonly Dictionary<HotKeyHandle, KeyCombination> _keyCombinationsByHandle = new();
    
    private int _suspensionCount;
    
    public bool IsSuspended => _suspensionCount > 0;
    
    public event EventHandler<HotKeyNativeRegistrationStateChangedEventArgs>? HotKeyNativeRegistrationStateChanged;
    
    public HotKeyRegistrationManager(IHotKeyService hotKeyService)
    {
        _hotKeyService = hotKeyService;
        _hotKeyService.HotKeyPressed += OnHotKeyPressed;
    }

    private void OnHotKeyPressed(object? sender, HotKeyPressedEventArgs e)
    {
        if (!_entries.TryGetValue(e.KeyCombination, out var entry))
            return;
        
        entry.Action();
    }

    private void SetState(KeyCombination keyCombination, HotKeyNativeRegistrationState state)
    {
        var entry = _entries[keyCombination];
        if (entry.State == state)
            return;
        
        _entries[keyCombination] = entry with { State = state };
        
        HotKeyNativeRegistrationStateChanged?.Invoke(
            this, 
            new HotKeyNativeRegistrationStateChangedEventArgs(entry.Token, state)
            );
    }

    private HotKeyNativeRegistrationState RegisterCore(KeyCombination keyCombination)
    {
        var registerResult = _hotKeyService.Register(keyCombination);
        HotKeyNativeRegistrationState state = registerResult.IsSuccess
            ? HotKeyNativeRegistrationSuccess.Instance
            : new HotKeyNativeRegistrationFailed(registerResult.Error);
        SetState(keyCombination, state);
        return state;
    }
    
    public HotKeyInternalRegistrationResult Register(KeyCombination keyCombination, Action action)
    {
        if (keyCombination.Key == Key.None)
            throw new ArgumentException("Key combination must be set", nameof(keyCombination));
        
        if (IsSuspended)
            throw new InvalidOperationException("Cannot register hot key while suspended");

        if (IsBound(keyCombination))
            return HotKeyInternalRegistrationConflict.Instance;

        var handle = new HotKeyHandle();
        var token = new HotKeyRegistrationToken();
        _entries[keyCombination] = new HotKeyEntry(handle, token, HotKeyNoNativeRegistration.Instance, action);
        _keyCombinationsByHandle[handle] = keyCombination;
        
        var state = RegisterCore(keyCombination);
        return new HotKeyInternalRegistrationSuccess(handle, token, state);
    }

    public void Unregister(HotKeyHandle handle)
    {
        if (!_keyCombinationsByHandle.TryGetValue(handle, out var keyCombination))
            return;
        
        var entry = _entries[keyCombination];
        
        if (entry.State is HotKeyNativeRegistrationSuccess && !IsSuspended)
            _hotKeyService.Unregister(keyCombination);
        
        _entries.Remove(keyCombination);
        _keyCombinationsByHandle.Remove(handle);
        
        HotKeyNativeRegistrationStateChanged?.Invoke(
            this, 
            new HotKeyNativeRegistrationStateChangedEventArgs(entry.Token, HotKeyNoNativeRegistration.Instance)
            );
    }
    
    public bool IsBound(KeyCombination keyCombination) => _entries.ContainsKey(keyCombination);

    public HotKeyNativeRegistrationState GetState(KeyCombination keyCombination) => 
        _entries.GetValueOrDefault(keyCombination)?.State ?? HotKeyNoNativeRegistration.Instance;

    private sealed class SuspensionLease(HotKeyRegistrationManager manager) : IDisposable
    {
        private bool _disposed;
        
        public void Dispose()
        {
            if (_disposed)
                return;
            
            _disposed = true;
            
            manager.ReleaseSuspension();
        }
    }
    
    public IDisposable Suspend()
    {
        ++_suspensionCount;
        if (_suspensionCount != 1) 
            return new SuspensionLease(this);
        
        foreach (var (keyCombination, entry) in _entries)
        {
            if (entry.State is HotKeyNativeRegistrationSuccess)
                _hotKeyService.Unregister(keyCombination);
        }

        return new SuspensionLease(this);
    }

    private void ReleaseSuspension()
    {
        if (_suspensionCount <= 0)
            throw new InvalidOperationException("No active suspension");
        
        --_suspensionCount;
        if (_suspensionCount == 0)
            RestoreRegistrations();
    }
    
    private void RestoreRegistrations()
    {
        foreach (var keyCombination in _entries.Keys.ToList())
            RegisterCore(keyCombination);
    }
}