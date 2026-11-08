using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using D2RInstanceManager.Config.Exceptions;
using Microsoft.Extensions.Logging;

namespace D2RInstanceManager.Config.Stores;

public class JsonConfigStore(ConfigEnvironment configEnvironment, ILogger<JsonConfigStore> logger) : IConfigStore
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new()
    {
        WriteIndented = true,
        RespectNullableAnnotations = true,
    };

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, JsonSerializerOptions);
        Directory.CreateDirectory(configEnvironment.DirectoryPath);
        await File.WriteAllTextAsync(configEnvironment.TempFilePath, json, cancellationToken);

        File.Move(configEnvironment.TempFilePath, configEnvironment.FilePath, true);
        
        logger.LogInformation("Saved config to {FilePath}", configEnvironment.FilePath);
    }

    public async Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default)
    { 
        var json = await File.ReadAllTextAsync(configEnvironment.FilePath, cancellationToken);
        
        var config = JsonSerializer.Deserialize<AppConfig>(json, JsonSerializerOptions) 
                     ?? throw new InvalidConfigException("Config is null");
        
        logger.LogInformation("Loaded config from {FilePath}", configEnvironment.FilePath);
        
        return config;
    }
}