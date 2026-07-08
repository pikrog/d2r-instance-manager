using System;
using System.IO;

namespace AvaloniaApplication1.Config;

public class ConfigEnvironment
{
    public required string FilePath { get; init; }

    public required string DirectoryPath { get; init; }

    public static ConfigEnvironment CreateInApplicationDataDirectory()
    {
        var applicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDirectoryPath = Path.Combine(applicationDataDirectory, ConfigConstants.ConfigDirectoryName);
        var configFilePath = Path.Combine(configDirectoryPath, ConfigConstants.ConfigFileName);
        return new ConfigEnvironment
        {
            DirectoryPath = configDirectoryPath,
            FilePath = configFilePath
        };
    }
}