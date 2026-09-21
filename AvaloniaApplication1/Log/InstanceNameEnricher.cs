using System;
using Serilog.Core;
using Serilog.Events;

namespace AvaloniaApplication1.Log;

public class InstanceNameEnricher(InstanceNameRegistry registry) : ILogEventEnricher
{
    public void Enrich(LogEvent logEvent, ILogEventPropertyFactory propertyFactory)
    {
        if (!logEvent.Properties.TryGetValue(LogProperties.InstanceId, out var value))
            return;

        if (value is not ScalarValue { Value: Guid id })
            return;

        var name = registry.Get(id);
        if (name is null)
            return;

        logEvent.AddPropertyIfAbsent(propertyFactory.CreateProperty(LogProperties.InstanceName, name));
    }
}
