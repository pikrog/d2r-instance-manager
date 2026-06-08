using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Exceptions;

namespace AvaloniaApplication1.Config;

public class JsonConfigStore(AppEnvironment appEnvironment) : IConfigStore
{
    //public string FilePath { get; } = appEnvironment.ConfigFilePath;
    
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, JsonSerializerOptions);
        Directory.CreateDirectory(appEnvironment.ConfigDirectoryPath);
        await File.WriteAllTextAsync(appEnvironment.ConfigFilePath, json, cancellationToken);
    }

    public async Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(appEnvironment.ConfigFilePath, cancellationToken);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? throw new InvalidConfigException("Config is null");
    }
}