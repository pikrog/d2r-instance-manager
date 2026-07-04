using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using AvaloniaApplication1.Providers.GameExecutablePath;
using Microsoft.Win32;

namespace AvaloniaApplication1.Services;

public class GameExecutablePathLocator(IEnumerable<IGameExecutablePathProvider> providers)
{
    public string? TryLocate() => providers.Select(provider => provider.TryGet()).OfType<string>().FirstOrDefault();
}