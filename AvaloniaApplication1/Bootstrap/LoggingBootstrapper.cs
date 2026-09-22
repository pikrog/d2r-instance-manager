using System;
using AvaloniaApplication1.Log;
using Serilog;
using Serilog.Templates;

namespace AvaloniaApplication1.Bootstrap;

public static class LoggingBootstrapper
{
    private const string ExpressionTemplateString = "[{@t:HH:mm:ss.fff}] [{@l:u3}] [{SourceContext}] " +
                                                    "{#if InstanceName is not null}[{InstanceName}] {#end}" +
                                                    "{@m}\r\n";
    
    public static CoreLoggingServicesBundle Bootstrap(AppDataEnvironment environment)
    {
        var logEnvironment = LogEnvironment.Derive(environment);
        return Bootstrap(logEnvironment);
    }
    
    public static CoreLoggingServicesBundle Bootstrap(LogEnvironment logEnvironment)
    {
        var logStore = new LogStore();
        
        var instanceNameRegistry = new InstanceNameRegistry();
        var instanceNameEnricher = new InstanceNameEnricher(instanceNameRegistry);

        var inMemorySink = new InMemoryLogSink(logStore);

        var expressionTemplate = new ExpressionTemplate(ExpressionTemplateString);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Verbose()
            .Enrich.With(instanceNameEnricher)
            .WriteTo.Sink(inMemorySink)
            .WriteTo.Console(expressionTemplate)
            .WriteTo.File(expressionTemplate, logEnvironment.FilePath, rollingInterval: RollingInterval.Day)
            .CreateLogger();

        Serilog.Log.Logger = logger;

        return new CoreLoggingServicesBundle(logger, logStore, instanceNameRegistry);
    }
}