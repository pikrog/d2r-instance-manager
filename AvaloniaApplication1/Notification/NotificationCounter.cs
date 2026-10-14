using System;
using System.Threading;

namespace AvaloniaApplication1.Notification;

public class NotificationCounter : INotificationSource, INotificationSink, INotificationAcknowledger
{
    private readonly Lock _gate = new();

    private bool _isAcknowledging;

    private int _count;

    public int Count
    {
        get
        {
            lock (_gate)
                return _count;
        }
    }
    public event EventHandler<int>? CountChanged;

    public void Increment()
    {
        int newCount;

        lock (_gate)
        {
            if (_isAcknowledging)
                return;
            
            newCount = ++_count;
        }
        
        CountChanged?.Invoke(this, newCount);
    }
    
    public void SetAcknowledgingNotifications(bool isAcknowledging)
    {
        bool notify;

        lock (_gate)
        {
            if (_isAcknowledging == isAcknowledging)
                return;
            
            _isAcknowledging = isAcknowledging;

            if (!isAcknowledging)
                return;

            notify = _count != 0;
            _count = 0;
        }
        
        if (notify)
            CountChanged?.Invoke(this, 0);
    }
}