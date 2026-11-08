using System;
using System.Collections.ObjectModel;

namespace D2RInstanceManager.Toast;

public class ToastHost : IToastHost, IToastController
{
    private class SuspensionLease(ToastHost host) : IDisposable
    {
        private bool _isDisposed;

        public void Dispose()
        {
            if (_isDisposed)
                return;

            _isDisposed = true;

            host.ResumeToasts();
        }
    }
    
    private readonly ObservableCollection<ToastNotification> _toasts = [];
    
    public ReadOnlyObservableCollection<ToastNotification> Toasts { get; }

    private int _suspensionCount;

    public ToastHost()
    {
        Toasts = new ReadOnlyObservableCollection<ToastNotification>(_toasts);
    }

    public void Add(ToastNotification toastNotification)
    {
        if (_toasts.Contains(toastNotification))
            return;

        toastNotification.CloseRequested += OnCloseRequested;
        _toasts.Insert(0, toastNotification);
        toastNotification.Start();
    }

    private void OnCloseRequested(object? sender, EventArgs e)
    {
        if (sender is ToastNotification toast)
            Remove(toast);
    }

    private void Remove(ToastNotification toastNotification)
    {
        if (!_toasts.Remove(toastNotification))
            return;
        
        toastNotification.CloseRequested -= OnCloseRequested;
        toastNotification.Dispose();
    }
    
    public IDisposable SuspendToasts()
    {
        if (_suspensionCount == 0)
        {
            foreach (var toast in _toasts)
                toast.Suspend();
        }
        
        _suspensionCount++;

        return new SuspensionLease(this);
    }

    private void ResumeToasts()
    {
        if (_suspensionCount <= 0)
            throw new InvalidOperationException("No suspensions");
        
        _suspensionCount--;

        if (_suspensionCount > 0)
            return;
        
        foreach (var toast in _toasts)
            toast.Resume();
    }
}