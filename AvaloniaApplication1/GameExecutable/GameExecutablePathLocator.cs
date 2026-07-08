using System.Collections.Generic;
using System.Linq;
using AvaloniaApplication1.GameExecutable.Providers;

namespace AvaloniaApplication1.GameExecutable;

public class GameExecutablePathLocator(IEnumerable<IGameExecutablePathProvider> providers)
{
    public string? TryLocate() => providers.Select(provider => provider.TryGet()).OfType<string>().FirstOrDefault();
}