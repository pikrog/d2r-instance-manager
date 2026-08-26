using System;
using System.Runtime.CompilerServices;
using Avalonia;
using AvaloniaApplication1.Controls;
using AvaloniaApplication1.HotKey.Platform;

namespace AvaloniaApplication1.Common;

public static class HotKeySuspension
{
    public static readonly AttachedProperty<IHotKeySuspensionCoordinator?> CoordinatorProperty =
        AvaloniaProperty.RegisterAttached<HotkeyBox, IHotKeySuspensionCoordinator?>("Coordinator",
            typeof(HotKeySuspension));
    
    public static IHotKeySuspensionCoordinator? GetCoordinator(HotkeyBox element) => element.GetValue(CoordinatorProperty);
    
    public static void SetCoordinator(HotkeyBox element, IHotKeySuspensionCoordinator? value) => element.SetValue(CoordinatorProperty, value);

    static HotKeySuspension()
    {
        CoordinatorProperty.Changed.AddClassHandler<HotkeyBox>(OnCoordinatorChanged);
    }
    
    private static readonly ConditionalWeakTable<HotkeyBox, State> States = new();

    private sealed class State : IDisposable
    {
        public IDisposable? IsCapturingSubscription;

        public IDisposable? Suspension;
        
        public void Dispose()
        {
            IsCapturingSubscription?.Dispose();
            Suspension?.Dispose();
        }
    }

    private static void OnCoordinatorChanged(HotkeyBox control, AvaloniaPropertyChangedEventArgs e)
    {
        if (States.TryGetValue(control, out var oldState))
        {
            oldState.Dispose();
            States.Remove(control);
        }

        var coordinator = e.NewValue as IHotKeySuspensionCoordinator;
        if (coordinator is null)
            return;

        var state = new State();
        States.Add(control, state);

        state.IsCapturingSubscription = control.GetObservable(HotkeyBox.IsCapturingProperty)
            .Subscribe(isCapturing =>
            {
                if (isCapturing)
                {
                    state.Suspension = coordinator.Suspend();
                }
                else
                {
                    state.Suspension?.Dispose();
                    state.Suspension = null;
                }
            });

        control.DetachedFromVisualTree += (_, _) =>
        {
            if (!States.TryGetValue(control, out var s)) 
                return;
            
            s.Dispose();
            States.Remove(control);
        };
    }
}