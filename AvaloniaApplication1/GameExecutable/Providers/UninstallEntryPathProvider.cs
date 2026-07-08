using System.Diagnostics.CodeAnalysis;
using System.IO;
using Microsoft.Win32;

namespace AvaloniaApplication1.GameExecutable.Providers;

[SuppressMessage("Interoperability", "CA1416")]
public class UninstallEntryPathProvider : IGameExecutablePathProvider
{
    private const string UninstallEntriesKeyPath = @"SOFTWARE\WOW6432Node\Microsoft\Windows\CurrentVersion\Uninstall";
    private const string InstallLocationValueName = "InstallLocation";
    private const string DisplayIconValueName = "DisplayIcon";

    public string? TryGet()
    {
        var uninstallEntry = Path.Combine(UninstallEntriesKeyPath, GameExecutableConstants.UninstallEntrySubKey);
        using var key = Registry.LocalMachine.OpenSubKey(uninstallEntry);
        if (key is null)
            return null;

        var installLocation = key.GetValue(InstallLocationValueName) as string;
        if (!string.IsNullOrWhiteSpace(installLocation))
        {
            var path = Path.Combine(installLocation, GameExecutableConstants.ExecutableName);
            if (File.Exists(path))
                return path;
        }

        var iconPath = key.GetValue(DisplayIconValueName) as string;
        if (!string.IsNullOrWhiteSpace(iconPath) && File.Exists(iconPath))
            return iconPath;

        return null;
    }
}