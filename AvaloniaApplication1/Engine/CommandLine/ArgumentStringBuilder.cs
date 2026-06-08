using System.Collections.Generic;

namespace AvaloniaApplication1.Engine.CommandLine;

public class ArgumentStringBuilder(IArgumentListFormatter argumentListFormatter, string argumentSeparator = " ") : IArgumentStringBuilder
{
    public string ArgumentSeparator { get; } = argumentSeparator;

    public IArgumentListFormatter ArgumentListFormatter { get; } = argumentListFormatter;

    public ArgumentStringBuilder(IArgumentFormatter argumentFormatter, string argumentSeparator = " ")
        : this(new ArgumentListFormatter(argumentFormatter), argumentSeparator)
    {
    }

    public string Build(IEnumerable<Argument> arguments)
    {
        var formatted = ArgumentListFormatter.Format(arguments);
        return string.Join(ArgumentSeparator, formatted);
    }
}