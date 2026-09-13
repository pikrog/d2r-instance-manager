using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config.Exceptions;
using Microsoft.Extensions.Logging;

namespace AvaloniaApplication1.Config.Stores;

public class JsonConfigStore(ConfigEnvironment configEnvironment, ILogger<JsonConfigStore> logger) : IConfigStore
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, JsonSerializerOptions);
        Directory.CreateDirectory(configEnvironment.DirectoryPath);
        await File.WriteAllTextAsync(configEnvironment.FilePath, json, cancellationToken);
        
        logger.LogInformation("Saved config to {FilePath}", configEnvironment.FilePath);
    }

    public async Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(configEnvironment.FilePath, cancellationToken);
        
        logger.LogInformation("Loaded config from {FilePath}", configEnvironment.FilePath);
        
        return JsonSerializer.Deserialize<AppConfig>(json) ?? throw new InvalidConfigException("Config is null");
    }
}