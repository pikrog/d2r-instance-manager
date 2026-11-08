using System;
using System.IO;

namespace D2RInstanceManager;

public class AppDataEnvironment
{
    private const string AppDataFolderName = "D2R Instance Manager";
    
    public required string DirectoryPath { get; init; }

    public static AppDataEnvironment CreateDefault()
    {
        var appDataFolderPath = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        var rootPath = Path.Combine(appDataFolderPath, AppDataFolderName);
        return new AppDataEnvironment
        {
            DirectoryPath = rootPath
        };
    }
}