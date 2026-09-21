using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication1.Engine.CommandLine;

public class ArgumentStringBuilder(IArgumentFormatter argumentFormatter, string argumentSeparator = " ") : IArgumentStringBuilder
{
    public string ArgumentSeparator { get; } = argumentSeparator;

    public string Build(IEnumerable<Argument> arguments)
    {
        var formatted = arguments.Select(arg =>
            arg.IsParameter
                ? argumentFormatter.Parameter(arg.Name, arg.Value!)
                : argumentFormatter.Flag(arg.Name)
        );
        return string.Join(ArgumentSeparator, formatted);
    }
}