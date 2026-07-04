using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AvaloniaApplication1.Providers.GameExecutablePath;

[SuppressMessage("Interoperability", "CA1416")]
public class ProgramFilesPathProvider : IGameExecutablePathProvider
{
    private const string GameExecutableName = "D2R.exe";
    private const string GameFolderName = "Diablo II Resurrected";

    public string? TryGet()
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var path = Path.Combine(programFiles, GameFolderName, GameExecutableName);
        return File.Exists(path) ? path : null;
    }
}