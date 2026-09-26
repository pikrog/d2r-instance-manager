using AvaloniaApplication1.Log;
using AvaloniaApplication1.Log.Instance;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public record CoreLoggingServicesBundle(
    ILogger Logger,
    LogStore LogStore,
    InstanceNameRegistry InstanceNameRegistry,
    LogSettings LogSettings);