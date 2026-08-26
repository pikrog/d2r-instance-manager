using Avalonia.Input;
using AvaloniaApplication1.HotKey.Platform.Registration.Native;

namespace AvaloniaApplication1.HotKey.Coordination;

public sealed record HotKeyBindingSummary(HotKeyBindingState State, KeyCombination KeyCombination, HotKeyNativeRegistrationError? Error)
{
    public static readonly HotKeyBindingSummary NotConfigured = 
        new(HotKeyBindingState.NotConfigured, new KeyCombination(Key.None, KeyModifiers.None), null);
}