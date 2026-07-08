using System;
using System.Diagnostics.CodeAnalysis;
using System.IO;

namespace AvaloniaApplication1.GameExecutable.Providers;

[SuppressMessage("Interoperability", "CA1416")]
public class ProgramFilesPathProvider : IGameExecutablePathProvider
{
    public string? TryGet()
    {
        var programFiles = Environment.GetFolderPath(Environment.SpecialFolder.ProgramFilesX86);
        var path = Path.Combine(programFiles, GameExecutableConstants.ProgramFilesDirectoryName, GameExecutableConstants.ExecutableName);
        return File.Exists(path) ? path : null;
    }
}