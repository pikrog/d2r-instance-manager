using System.IO;

namespace D2RInstanceManager.Config;

public class ConfigEnvironment
{
    private const string ConfigFileName = "config.json";
    
    private const string TempConfigFileName = $"{ConfigFileName}.tmp";
    
    public required string FilePath { get; init; }
    
    public required string TempFilePath { get; init; }
    
    public required string DirectoryPath { get; init; }

    public static ConfigEnvironment Derive(AppDataEnvironment environment)
    {
        var directoryPath = environment.DirectoryPath;
        var configFilePath = Path.Combine(directoryPath, ConfigFileName);
        var tempFilePath = Path.Combine(directoryPath, TempConfigFileName);
        
        return new ConfigEnvironment
        {
            DirectoryPath = directoryPath,
            FilePath = configFilePath,
            TempFilePath = tempFilePath
        };
    }
}