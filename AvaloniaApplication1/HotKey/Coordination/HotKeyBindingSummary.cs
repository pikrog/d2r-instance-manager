using Avalonia.Input;
using D2RInstanceManager.HotKey.Platform.Registration.Native;

namespace D2RInstanceManager.HotKey.Coordination;

public sealed record HotKeyBindingSummary(HotKeyBindingState State, KeyCombination KeyCombination, HotKeyNativeRegistrationError? Error)
{
    public static readonly HotKeyBindingSummary NotConfigured = 
        new(HotKeyBindingState.NotConfigured, new KeyCombination(Key.None, KeyModifiers.None), null);
}