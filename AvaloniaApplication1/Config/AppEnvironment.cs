using System;
using System.IO;

namespace AvaloniaApplication1.Config;

public class AppEnvironment
{
    public required string ConfigFilePath { get; init; }

    public required string ConfigDirectoryPath { get; init; }

    public static AppEnvironment CreateInApplicationDataDirectory()
    {
        var applicationDataDirectory = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var configDirectoryPath = Path.Combine(applicationDataDirectory, ConfigConstants.ConfigDirectoryName);
        var configFilePath = Path.Combine(configDirectoryPath, ConfigConstants.ConfigFileName);
        return new AppEnvironment
        {
            ConfigDirectoryPath = configDirectoryPath,
            ConfigFilePath = configFilePath
        };
    }
}