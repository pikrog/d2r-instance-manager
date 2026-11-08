using System;

namespace D2RInstanceManager.Notification;

public interface INotificationSource
{
    int Count { get; }
    event EventHandler<int>? CountChanged;
    void SetAcknowledgingNotifications(bool isAcknowledging);
}