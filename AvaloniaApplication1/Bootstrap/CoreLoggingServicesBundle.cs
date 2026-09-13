using AvaloniaApplication1.Log;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public record CoreLoggingServicesBundle(ILogger Logger, LogStore LogStore);