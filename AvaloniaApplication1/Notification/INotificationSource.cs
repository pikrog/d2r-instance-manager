using System;

namespace AvaloniaApplication1.Notification;

public interface INotificationSource
{
    int Count { get; }
    event EventHandler<int>? CountChanged;
    void SetAcknowledgingNotifications(bool isAcknowledging);
}