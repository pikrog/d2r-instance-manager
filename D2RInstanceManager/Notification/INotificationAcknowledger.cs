namespace D2RInstanceManager.Notification;

public interface INotificationAcknowledger
{
    void SetAcknowledgingNotifications(bool acknowledgingNotifications);
}