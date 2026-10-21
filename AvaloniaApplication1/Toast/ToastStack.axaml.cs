using System;
using Avalonia;
using Avalonia.Controls;

namespace AvaloniaApplication1.Toast;

public partial class ToastStack : UserControl
{
    public static readonly StyledProperty<IToastHost?> HostProperty =
        AvaloniaProperty.Register<ToastStack, IToastHost?>(nameof(Host));
    
    public IToastHost? Host
    {
        get => GetValue(HostProperty);
        set => SetValue(HostProperty, value);
    }
    
    private IDisposable? _suspension;
        
    public ToastStack()
    {
        InitializeComponent();
    }

    private void UpdateSuspension()
    {
        var shouldSuspend = IsPointerOver || IsKeyboardFocusWithin;

        if (shouldSuspend)
            _suspension ??= Host?.SuspendToasts();
        else
            ReleaseSuspension();
    }

    private void ReleaseSuspension()
    {
        _suspension?.Dispose();
        _suspension = null;
    }

    protected override void OnAttachedToVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnAttachedToVisualTree(e);
        UpdateSuspension();
    }

    protected override void OnDetachedFromVisualTree(VisualTreeAttachmentEventArgs e)
    {
        base.OnDetachedFromVisualTree(e);
        ReleaseSuspension();
    }

    protected override void OnPropertyChanged(AvaloniaPropertyChangedEventArgs change)
    {
        base.OnPropertyChanged(change);

        if (change.Property == IsPointerOverProperty || change.Property == IsKeyboardFocusWithinProperty)
        {
            UpdateSuspension();
        }
        else if (change.Property == HostProperty)
        {
            ReleaseSuspension();
            UpdateSuspension();
        }
    }
}