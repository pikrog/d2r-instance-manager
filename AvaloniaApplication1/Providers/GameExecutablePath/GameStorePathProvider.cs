using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Win32;

namespace AvaloniaApplication1.Providers.GameExecutablePath;

[SuppressMessage("Interoperability", "CA1416")]
public class GameStorePathProvider : IGameExecutablePathProvider
{
    private const string ChildrenKey = @"System\GameConfigStore\Children";

    private const string TitleIdValueName = "TitleId";
    private const string ExecutableValueName = "MatchedExeFullPath";

    private const string ExpectedTitleId = "1904560378";

    public string? TryGet()
    {
        using var root = Registry.CurrentUser.OpenSubKey(ChildrenKey);
        if (root is null)
            return null;

        foreach (var subKeyName in root.GetSubKeyNames())
        {
            using var subKey = root.OpenSubKey(subKeyName);
            if (subKey is null)
                continue;

            var titleId = subKey.GetValue(TitleIdValueName) as string;
            if (titleId != ExpectedTitleId)
                continue;

            var path = subKey.GetValue(ExecutableValueName) as string;
            if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                return path;
        }

        return null;
    }
}