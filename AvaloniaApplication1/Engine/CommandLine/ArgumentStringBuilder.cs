using System;
using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.Engine.Common;

namespace AvaloniaApplication1.Engine.CommandLine;

public class ArgumentStringBuilder(IArgumentFormatter argumentFormatter, string argumentSeparator = " ") : IArgumentStringBuilder
{
    public string ArgumentSeparator { get; } = argumentSeparator;

    private string Build(IEnumerable<Argument> arguments, bool doRedact)
    {
        var formatted = arguments.Select(arg => arg switch
        {
            FlagArgument a => argumentFormatter.Flag(a.Name),
            ParametrizedArgument a => argumentFormatter.Parameter(a.Name, a.Value),
            SensitiveParametrizedArgument a => 
                doRedact 
                ? argumentFormatter.Parameter(a.Name, RedactedString.RedactedValue)
                : argumentFormatter.Parameter(a.Name, a.Value.Reveal()),
            _ => throw new InvalidOperationException($"Unknown argument type: {arg}")
        });
        
        return string.Join(ArgumentSeparator, formatted);
    }

    public string Build(IEnumerable<Argument> arguments) => Build(arguments, false);

    public string BuildRedacted(IEnumerable<Argument> arguments) => Build(arguments, true);
}