using AvaloniaApplication1.Log;
using Serilog;

namespace AvaloniaApplication1.Bootstrap;

public static class LoggingBootstrapper
{
    public static CoreLoggingServicesBundle Bootstrap(AppDataEnvironment environment)
    {
        var logEnvironment = LogEnvironment.Derive(environment);
        return Bootstrap(logEnvironment);
    }
    
    public static CoreLoggingServicesBundle Bootstrap(LogEnvironment logEnvironment)
    {
        var logStore = new LogStore();
        
        var inMemorySink = new InMemoryLogSink(logStore);
        
        var logger = new LoggerConfiguration()
            .MinimumLevel.Debug()
            .WriteTo.Sink(inMemorySink)
            .WriteTo.Console()
            .WriteTo.File(logEnvironment.FilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();
        
        Serilog.Log.Logger = logger;
        
        return new CoreLoggingServicesBundle(logger, logStore);
    }
}