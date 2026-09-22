using System;
using System.IO;

namespace AvaloniaApplication1.Log;

public class LogEnvironment
{
    private const string DirectoryName = "logs";
    
    private const string TimestampFormat = "yyyy-MM-dd_HH-mm-ss-fff";
    
    private const string TimestampPlaceholder = "{Timestamp}";
    
    private const string FileNameBase = $"log-{TimestampPlaceholder}.txt";
    
    public required string FilePath { get; init; }
    
    public static LogEnvironment Derive(AppDataEnvironment environment)
    {
        var directoryPath = Path.Combine(environment.DirectoryPath, DirectoryName);
        var timestamp = DateTime.Now.ToString(TimestampFormat);
        var fileName = FileNameBase.Replace(TimestampPlaceholder, timestamp);
        var filePath = Path.Combine(directoryPath, fileName);
        
        return new LogEnvironment
        {
            FilePath = filePath
        };
    }
}