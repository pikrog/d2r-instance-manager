using System.IO;

namespace AvaloniaApplication1.Log;

public class LogEnvironment
{
    private const string DirectoryName = "logs";
    
    private const string FileName = "log-.txt";
    
    public required string FilePath { get; init; }
    
    public static LogEnvironment Derive(AppDataEnvironment environment)
    {
        var directoryPath = Path.Combine(environment.DirectoryPath, DirectoryName);
        var filePath = Path.Combine(directoryPath, FileName);
        return new LogEnvironment
        {
            FilePath = filePath
        };
    }
}