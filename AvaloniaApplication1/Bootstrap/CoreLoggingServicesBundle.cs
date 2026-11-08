using D2RInstanceManager.Log;
using D2RInstanceManager.Log.Instance;
using D2RInstanceManager.Notification;
using Serilog;

namespace D2RInstanceManager.Bootstrap;

public record CoreLoggingServicesBundle(
    ILogger Logger,
    LogStore LogStore,
    InstanceNameRegistry InstanceNameRegistry,
    LogSettings LogSettings,
    NotificationCounter NotificationCounter);