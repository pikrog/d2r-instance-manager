using System;
using System.Collections.Generic;
using Avalonia.Input;
using AvaloniaApplication1.HotKey;
using AvaloniaApplication1.HotKey.Registration;
using AvaloniaApplication1.Instance.Models.EventArgs;

namespace AvaloniaApplication1.Instance;

public class InstanceHotKeyManager
{
    private readonly HotKeyManager _hotKeyManager;
    
    private readonly InstanceService _instanceService;
    
    private readonly Dictionary<Guid, HotKeyRegistration> _registrations = new();

    private readonly Dictionary<KeyCombination, Guid> _instanceIdByHotKey = new();

    public event EventHandler<InstanceHotKeyRegistrationChangedEventArgs>? RegistrationChanged;
    
    public InstanceHotKeyManager(HotKeyManager hotKeyManager, InstanceService instanceService)
    {
        _hotKeyManager = hotKeyManager;
        _instanceService = instanceService;
        
        _hotKeyManager.RegistrationChanged += OnHotKeyRegistrationChanged;
        _instanceService.InstanceConfigChanged += OnInstanceConfigChanged;
    }

    private void OnHotKeyRegistrationChanged(object? sender, HotKeyRegistrationChangedEventArgs e)
    {
        if (!_instanceIdByHotKey.TryGetValue(e.KeyCombination, out var instanceId))
            return;

        if (!_registrations.TryGetValue(instanceId, out var registration))
            return;

        if (registration is HotKeyRegistrationConflict { Source: HotKeyConflictSource.Internal })
            return;

        _registrations[instanceId] = registration;
        RegistrationChanged?.Invoke(
            this, 
            new InstanceHotKeyRegistrationChangedEventArgs(instanceId)
            );
    }

    private void OnInstanceConfigChanged(object? sender, InstanceConfigChangedEventArgs e)
    {
        if (e.OldSnapshot is not null)
            Unregister(e.OldSnapshot.ShowCommandHotKey);
        
        if (e.NewSnapshot is not null)
            Register(e.NewSnapshot.ShowCommandHotKey, e.InstanceId);
    }

    public void Initialize()
    {
        foreach (var instance in _instanceService.GetConfigSnapshots())
            Register(instance.ShowCommandHotKey, instance.Id);
    }

    private void Register(KeyCombination keyCombination, Guid instanceId)
    {
        if (keyCombination.Key == Key.None)
            return;
        
        _registrations[instanceId] = _hotKeyManager.Register(
            keyCombination, 
            () => _instanceService.Show(instanceId)
            );
        
        _instanceIdByHotKey[keyCombination] = instanceId;
    }

    private void Unregister(KeyCombination keyCombination)
    {
        if (keyCombination.Key == Key.None)
            return;
        
        if (!_instanceIdByHotKey.TryGetValue(keyCombination, out var instanceId))
            return;
        
        _hotKeyManager.Unregister(keyCombination);
        
        _registrations.Remove(instanceId);
        _instanceIdByHotKey.Remove(keyCombination);
    }
    
    public HotKeyRegistration GetRegistration(Guid instanceId) => 
        _registrations.GetValueOrDefault(instanceId) ?? HotKeyRegistrationNotConfigured.Instance;
    
}