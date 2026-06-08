using System.Collections.Generic;

namespace AvaloniaApplication1.Engine.CommandLine;

public interface IArgumentStringBuilder
{
    string Build(IEnumerable<Argument> arguments);
}