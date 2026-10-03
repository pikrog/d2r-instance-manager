using AvaloniaApplication1.Log;
using AvaloniaApplication1.Log.Instance;
using Serilog;
using Serilog.Templates;

namespace AvaloniaApplication1.Bootstrap;

public static class LoggingBootstrapper
{
    private const string ExpressionTemplateString = "[{@t:HH:mm:ss.fff}] [{@l:u3}] " +
                                                    "{#if SourceContext is not null}[{SourceContext}] {#end}" +
                                                    "{#if InstanceName is not null}[{InstanceName}] {#end}" +
                                                    "{@m}\r\n" +
                                                    "{#if @x is not null}{@x}\r\n{#end}";
    
    public static CoreLoggingServicesBundle Bootstrap(AppDataEnvironment environment, LogSettings logSettings)
    {
        var logEnvironment = LogEnvironment.Derive(environment);
        return Bootstrap(logEnvironment, logSettings);
    }
    
    public static CoreLoggingServicesBundle Bootstrap(LogEnvironment logEnvironment, LogSettings logSettings)
    {
        var minimumLogLevel = LogEventLevelMapper.Map(logSettings.MinimumLevel);
        
        var logStore = new LogStore();
        
        var instanceNameRegistry = new InstanceNameRegistry();
        var instanceNameEnricher = new InstanceNameEnricher(instanceNameRegistry);

        var inMemorySink = new InMemoryLogSink(logStore);

        var expressionTemplate = new ExpressionTemplate(ExpressionTemplateString);

        var logger = new LoggerConfiguration()
            .MinimumLevel.Is(minimumLogLevel)
            .Enrich.With(instanceNameEnricher)
            .WriteTo.Sink(inMemorySink)
            .WriteTo.Console(expressionTemplate)
            .WriteTo.File(expressionTemplate, logEnvironment.FilePath)
            .CreateLogger();

        Serilog.Log.Logger = logger;

        return new CoreLoggingServicesBundle(logger, logStore, instanceNameRegistry, logSettings);
    }
}