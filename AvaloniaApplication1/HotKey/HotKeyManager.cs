using System;
using System.Collections.Generic;
using Avalonia.Input;
using AvaloniaApplication1.HotKey.Registration;

namespace AvaloniaApplication1.HotKey;

public class HotKeyManager : IHotKeySuspensionCoordinator
{
    private readonly IHotKeyService _hotKeyService;
    
    private readonly Dictionary<KeyCombination, Action> _bindings = new();
    
    private readonly Dictionary<KeyCombination, HotKeyRegistration> _registrations = new();
    
    private int _suspensionCount;
    
    public bool IsSuspended => _suspensionCount > 0;
    
    public event EventHandler<HotKeyRegistrationChangedEventArgs>? RegistrationChanged;
    
    public HotKeyManager(IHotKeyService hotKeyService)
    {
        _hotKeyService = hotKeyService;
        _hotKeyService.HotKeyPressed += OnHotKeyPressed;
    }

    private void OnHotKeyPressed(object? sender, HotKeyPressedEventArgs e)
    {
        if (!_bindings.TryGetValue(e.KeyCombination, out var action))
            return;
        
        action();
    }

    private void SetRegistration(KeyCombination keyCombination, HotKeyRegistration registration)
    {
        var previousRegistration = GetRegistration(keyCombination);
        if (Equals(previousRegistration, registration))
            return;
        
        _registrations[keyCombination] = registration;
        RegistrationChanged?.Invoke(this, new HotKeyRegistrationChangedEventArgs(keyCombination, registration));
    }
    
    private void RemoveRegistration(KeyCombination keyCombination)
    {
        var previousRegistration = GetRegistration(keyCombination);
        if (previousRegistration is HotKeyRegistrationNotConfigured)
            return;

        _registrations.Remove(keyCombination);

        RegistrationChanged?.Invoke(
            this,
            new HotKeyRegistrationChangedEventArgs(keyCombination, HotKeyRegistrationNotConfigured.Instance)
            );
    }

    private HotKeyRegistration RegisterCore(KeyCombination keyCombination)
    {
        if (!_hotKeyService.Register(keyCombination))
        {
            var conflictRegistration = new HotKeyRegistrationConflict(keyCombination, HotKeyConflictSource.External);
            SetRegistration(keyCombination, conflictRegistration);
            return conflictRegistration;
        }
        
        var successRegistration = new HotKeyRegistrationSuccess(keyCombination);
        SetRegistration(keyCombination, successRegistration);
        return successRegistration;
    }

    public HotKeyRegistration Register(KeyCombination keyCombination, Action action)
    {
        if (keyCombination.Key == Key.None)
            throw new ArgumentException("Key combination must be set", nameof(keyCombination));
        
        if (IsSuspended)
            throw new InvalidOperationException("Cannot register hot key while suspended");
        
        if (IsBound(keyCombination))
            return new HotKeyRegistrationConflict(keyCombination, HotKeyConflictSource.Internal);
        
        var registration = RegisterCore(keyCombination);

        _bindings[keyCombination] = action;
        
        return registration;
    }

    public void Unregister(KeyCombination keyCombination)
    {
        if (keyCombination.Key == Key.None)
            return;
        
        if (!_bindings.Remove(keyCombination))
            return;
        
        if (WasSuccessfullyRegisteredWithOs(keyCombination) && !IsSuspended)
            _hotKeyService.Unregister(keyCombination);
        
        RemoveRegistration(keyCombination);
    }
    
    public bool IsBound(KeyCombination keyCombination) => _bindings.ContainsKey(keyCombination);
    
    public HotKeyRegistration GetRegistration(KeyCombination keyCombination) => 
        _registrations.GetValueOrDefault(keyCombination) ?? HotKeyRegistrationNotConfigured.Instance;
    
    private bool WasSuccessfullyRegisteredWithOs(KeyCombination keyCombination) => 
        WasSuccessfullyRegisteredWithOs(GetRegistration(keyCombination));

    private static bool WasSuccessfullyRegisteredWithOs(HotKeyRegistration registration) => 
        registration is HotKeyRegistrationSuccess;

    private sealed class SuspensionLease(HotKeyManager manager) : IDisposable
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
        
        foreach (var (keyCombination, registration) in _registrations)
        {
            if (WasSuccessfullyRegisteredWithOs(registration))
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
        foreach (var (keyCombination, _) in _bindings)
            RegisterCore(keyCombination);
    }
}