using System.Collections.Generic;
using System.Linq;
using D2RInstanceManager.GameExecutable.Providers;

namespace D2RInstanceManager.GameExecutable;

public class GameExecutablePathLocator(IEnumerable<IGameExecutablePathProvider> providers) : IGameExecutablePathLocator
{
    public string? TryLocate() => providers.Select(provider => provider.TryGet()).OfType<string>().FirstOrDefault();
}