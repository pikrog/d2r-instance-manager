using System;
using System.Diagnostics;
using Avalonia.Threading;
using D2RInstanceManager.Toast.Descriptors;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;

namespace D2RInstanceManager.Toast;

public sealed partial class ToastNotification : ObservableObject, IDisposable
{
    public static readonly TimeSpan DecayDuration = TimeSpan.FromMilliseconds(200);
    
    private static readonly TimeSpan PostDecayDuration = TimeSpan.FromMilliseconds(200);

    private static readonly TimeSpan CloseDelay = DecayDuration + PostDecayDuration;
    
    private readonly Stopwatch _stopwatch = new();

    private readonly DispatcherTimer _timer = new() { Interval = TimeSpan.FromMilliseconds(100) };
    
    private bool _isStarted;

    private readonly TimeSpan _duration;

    private bool _isSuspended;
    
    private IDisposable? _decayDisposable;
    
    private bool _isClosed;
    
    private bool _isDisposed;
    
    public ToastDescriptor Descriptor { get; }
    
    [ObservableProperty]
    public partial bool IsDecaying { get; private set; }
    
    public EventHandler? CloseRequested;

    public ToastNotification(ToastDescriptor descriptor)
    {
        Descriptor = descriptor;

        _duration = ToastDurations.Get(descriptor.Severity);

        _timer.Tick += OnTick;
    }

    private void OnTick(object? sender, EventArgs e)
    {
        if (_stopwatch.Elapsed < _duration)
            return;

        Decay();
    }

    public void Start()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        
        if (_isStarted)
            return;
        
        _isStarted = true;

        if (_isSuspended || IsDecaying || _isClosed)
            return;
        
        _stopwatch.Start();
        _timer.Start();
    }

    public void Suspend()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        
        _isSuspended = true;

        if (!_isStarted)
            return;
        
        _stopwatch.Stop();
        _timer.Stop();
    }

    public void Resume()
    {
        ObjectDisposedException.ThrowIf(_isDisposed, this);
        
        _isSuspended = false;

        if (!_isStarted || IsDecaying || _isClosed)
            return;
        
        _stopwatch.Start();
        _timer.Start();
    }

    private void Decay()
    {
        if (IsDecaying || _isDisposed)
            return;
        
        IsDecaying = true;
        
        _stopwatch.Stop();
        _timer.Stop();
        
        _decayDisposable = DispatcherTimer.RunOnce(Close, CloseDelay);
    }
    
    [RelayCommand]
    public void Close()
    {
        if (_isClosed || _isDisposed)
            return;
        
        _isClosed = true;
        
        CloseRequested?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        if (_isDisposed)
            return;
        
        _isDisposed = true;
        
        _timer.Stop();
        _timer.Tick -= OnTick;
        
        _stopwatch.Stop();
        
        _decayDisposable?.Dispose();
    }
}