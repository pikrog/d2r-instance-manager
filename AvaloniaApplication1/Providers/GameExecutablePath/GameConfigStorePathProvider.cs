using System.Diagnostics.CodeAnalysis;
using System.IO;
using AvaloniaApplication1.Constants;
using Microsoft.Win32;

namespace AvaloniaApplication1.Providers.GameExecutablePath;

[SuppressMessage("Interoperability", "CA1416")]
public class GameConfigStorePathProvider : IGameExecutablePathProvider
{
    private const string GameConfigStoreChildrenKey = @"System\GameConfigStore\Children";

    private const string TitleIdValueName = "TitleId";
    private const string ExecutablePathValueName = "MatchedExeFullPath";

    public string? TryGet()
    {
        using var root = Registry.CurrentUser.OpenSubKey(GameConfigStoreChildrenKey);
        if (root is null)
            return null;

        foreach (var subKeyName in root.GetSubKeyNames())
        {
            using var subKey = root.OpenSubKey(subKeyName);
            if (subKey is null)
                continue;

            var titleId = subKey.GetValue(TitleIdValueName) as string;
            if (titleId != GameConstants.GameConfigStoreTitleId)
                continue;

            var path = subKey.GetValue(ExecutablePathValueName) as string;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                return path;
        }

        return null;
    }
}