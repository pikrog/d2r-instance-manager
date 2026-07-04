using System.Diagnostics.CodeAnalysis;
using System.IO;
using AvaloniaApplication1.Config;
using Microsoft.Win32;

namespace AvaloniaApplication1.Providers.GameExecutablePath;

[SuppressMessage("Interoperability", "CA1416")]
public class UninstallEntryPathProvider : IGameExecutablePathProvider
{
    private const string KeyPath =
        @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall\Diablo II Resurrected";

    private const string InstallLocationValueName = "InstallLocation";
    private const string DisplayIconValueName = "DisplayIcon";
    private const string GameExecutableName = "D2R.exe";

    public string? TryGet()
    {
        using var key = Registry.LocalMachine.OpenSubKey(KeyPath);
        if (key is null)
            return null;

        var installLocation = key.GetValue(InstallLocationValueName) as string;
        if (!string.IsNullOrWhiteSpace(installLocation))
        {
            var path = Path.Combine(installLocation, GameExecutableName);
            if (File.Exists(path))
                return path;
        }

        var iconPath = key.GetValue(DisplayIconValueName) as string;
        if (!string.IsNullOrWhiteSpace(iconPath) && File.Exists(iconPath))
            return iconPath;

        return null;
    }
}