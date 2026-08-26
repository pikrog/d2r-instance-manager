using System;
using System.Collections.Generic;
using AvaloniaApplication1.HotKey.Platform;
using AvaloniaApplication1.HotKey.Platform.Registration.Internal;
using AvaloniaApplication1.HotKey.Platform.Registration.Native;

namespace AvaloniaApplication1.HotKey.Coordination;

public abstract class HotKeyBindingCoordinator<THotKeyCommand> where THotKeyCommand : HotKeyCommand
{
    private readonly HotKeyRegistrationManager _manager;

    private sealed record HotKeyRegistration(HotKeyHandle Handle, HotKeyRegistrationToken Token);
    
    private sealed record HotKeyEntry(
        HotKeyBindingState State,
        KeyCombination KeyCombination,
        HotKeyRegistration? Registration,
        HotKeyNativeRegistrationError? Error
        );

    private readonly Dictionary<THotKeyCommand, HotKeyEntry> _entries = new();
    
    private readonly Dictionary<HotKeyRegistrationToken, THotKeyCommand> _commandsByToken = new();
    
    public event EventHandler<HotKeyBindingStateChangedEventArgs<THotKeyCommand>>? BindingStateChanged;

    protected HotKeyBindingCoordinator(HotKeyRegistrationManager manager)
    {
        _manager = manager;
        _manager.HotKeyNativeRegistrationStateChanged += OnHotKeyNativeRegistrationStateChanged;
    }

    private void OnHotKeyNativeRegistrationStateChanged(object? sender, HotKeyNativeRegistrationStateChangedEventArgs e)
    {
        if (!_commandsByToken.TryGetValue(e.Token, out var command)) 
            return;
        
        if (!_entries.TryGetValue(command, out var entry)) 
            return;
        
        var (newState, newError) = ResolveBindingState(e.State);
        _entries[command] = entry with
        {
            State = newState, 
            Error = newError
        };
        
        BindingStateChanged?.Invoke(
            this, 
            new HotKeyBindingStateChangedEventArgs<THotKeyCommand>(command, newState)
            );
    }

    protected abstract Action CreateAction(THotKeyCommand command);

    private (HotKeyBindingState, HotKeyNativeRegistrationError?) ResolveBindingState(
        HotKeyNativeRegistrationState nativeRegistrationState)
    {
        var newState = nativeRegistrationState switch
        {
            HotKeyNativeRegistrationSuccess => HotKeyBindingState.Registered,
            
            HotKeyNativeRegistrationFailed f => 
                f.Error.IsAlreadyRegistered 
                    ? HotKeyBindingState.ExternalConflict 
                    : HotKeyBindingState.UnknownError,
            
            HotKeyNoNativeRegistration => HotKeyBindingState.NotConfigured,
            
            _ => throw new InvalidOperationException($"Unexpected native registration state: {nativeRegistrationState}")
        };
        var error = (nativeRegistrationState as HotKeyNativeRegistrationFailed)?.Error;
        return (newState, error);
    }

    protected virtual void Register(THotKeyCommand command, KeyCombination keyCombination)
    {
        var action = CreateAction(command);
        
        var registrationResult = _manager.Register(keyCombination, action);

        HotKeyRegistration? registration = null;
        if (registrationResult is HotKeyInternalRegistrationSuccess { Handle: { } handle, Token: { } token })
        {
            registration = new HotKeyRegistration(handle, token);
            _commandsByToken[token] = command;
        }

        var (bindingState, nativeError) = registrationResult switch
        {
            HotKeyInternalRegistrationSuccess r => ResolveBindingState(r.State),
            HotKeyInternalRegistrationConflict => (HotKeyBindingState.InternalConflict, null),
            _ => throw new InvalidOperationException($"Unexpected registration result: {registrationResult}")
        };
        
        var entry = new HotKeyEntry(bindingState, keyCombination, registration, nativeError);
        _entries[command] = entry;
        
        BindingStateChanged?.Invoke(
            this,
            new HotKeyBindingStateChangedEventArgs<THotKeyCommand>(command, bindingState)
            );
    }

    protected void Unregister(THotKeyCommand command)
    {
        if (!_entries.TryGetValue(command, out var entry)) 
            return;

        if (entry.Registration is { } registration)
        {
            _manager.Unregister(registration.Handle);
            _commandsByToken.Remove(registration.Token);
        }

        _entries.Remove(command);
        
        BindingStateChanged?.Invoke(
            this,
            new HotKeyBindingStateChangedEventArgs<THotKeyCommand>(command, HotKeyBindingState.NotConfigured)
            );
    }

    public HotKeyBindingSummary GetBindingSummary(THotKeyCommand command) =>
        _entries.TryGetValue(command, out var entry)
            ? new HotKeyBindingSummary(entry.State, entry.KeyCombination, entry.Error)
            : HotKeyBindingSummary.NotConfigured;
}

public abstract class HotKeyBindingCoordinator<THotKeyCommand, THotKeyCommandId>(HotKeyRegistrationManager manager)
    : HotKeyBindingCoordinator<THotKeyCommand>(manager)
    where THotKeyCommand : HotKeyCommand
    where THotKeyCommandId : notnull
{
    private readonly Dictionary<THotKeyCommandId, THotKeyCommand> _commandsById = new();

    protected abstract THotKeyCommandId GetCommandId(THotKeyCommand command);

    protected override void Register(THotKeyCommand command, KeyCombination keyCombination)
    {
        base.Register(command, keyCombination);

        var id = GetCommandId(command);
        _commandsById[id] = command;
    }

    protected void Unregister(THotKeyCommandId id)
    {
        if (!_commandsById.TryGetValue(id, out var command))
            return;

        Unregister(command);
        _commandsById.Remove(id);
    }

    public HotKeyBindingSummary GetBindingSummary(THotKeyCommandId id) =>
        _commandsById.TryGetValue(id, out var command)
            ? GetBindingSummary(command)
            : HotKeyBindingSummary.NotConfigured;
}