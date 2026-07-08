using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Win32;

namespace AvaloniaApplication1.GameExecutable.Providers;

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
            if (titleId != GameExecutableConstants.GameConfigStoreTitleId)
                continue;

            var path = subKey.GetValue(ExecutablePathValueName) as string;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                return path;
        }

        return null;
    }
}