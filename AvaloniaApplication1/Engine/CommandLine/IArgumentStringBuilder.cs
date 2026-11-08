using System.Collections.Generic;

namespace D2RInstanceManager.Engine.CommandLine;

public interface IArgumentStringBuilder
{
    string Build(IEnumerable<Argument> arguments);
    string BuildRedacted(IEnumerable<Argument> arguments);
}