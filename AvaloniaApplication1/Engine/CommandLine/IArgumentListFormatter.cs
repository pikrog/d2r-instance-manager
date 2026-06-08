using System.Collections.Generic;

namespace AvaloniaApplication1.Engine.CommandLine;

public interface IArgumentListFormatter
{
    IEnumerable<string> Format(IEnumerable<Argument> arguments);
}