using System;
using System.IO;

namespace AvaloniaApplication1.Config;

public class ConfigEnvironment
{
    private const string ConfigFileName = "config.json";
    
    public required string FilePath { get; init; }
    
    public required string DirectoryPath { get; init; }

    public static ConfigEnvironment Derive(AppDataEnvironment environment)
    {
        var directoryPath = environment.DirectoryPath;
        var configFilePath = Path.Combine(directoryPath, ConfigFileName);
        
        return new ConfigEnvironment
        {
            DirectoryPath = directoryPath,
            FilePath = configFilePath
        };
    }
}