using System.Collections.Generic;
using System.Linq;

namespace AvaloniaApplication1.Engine.CommandLine;

public class ArgumentListFormatter(IArgumentFormatter argumentFormatter) : IArgumentListFormatter
{
    public IArgumentFormatter ArgumentFormatter { get; } = argumentFormatter;

    public IEnumerable<string> Format(IEnumerable<Argument> arguments)
    {
        return arguments.Select(arg =>
            arg.IsParameter
                ? ArgumentFormatter.Parameter(arg.Name, arg.Value!)
                : ArgumentFormatter.Flag(arg.Name)
        );
    }
}