using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using AvaloniaApplication1.Constants;

namespace AvaloniaApplication1.Providers.GameExecutablePath;

[SuppressMessage("Interoperability", "CA1416")]
public class ProgramFilesPathProvider : IGameExecutablePathProvider
{
    public string? TryGet()
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var path = Path.Combine(programFiles, GameConstants.ProgramFilesDirectoryName, GameConstants.ExecutableName);
        return File.Exists(path) ? path : null;
    }
}