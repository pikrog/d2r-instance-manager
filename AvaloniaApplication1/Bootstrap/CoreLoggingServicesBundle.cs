using AvaloniaApplication1.Log;
using AvaloniaApplication1.Log.Instance;
using AvaloniaApplication1.Notification;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public record CoreLoggingServicesBundle(
    ILogger Logger,
    LogStore LogStore,
    InstanceNameRegistry InstanceNameRegistry,
    LogSettings LogSettings,
    NotificationCounter NotificationCounter);