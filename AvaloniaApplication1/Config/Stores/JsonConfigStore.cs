using System.IO;
using System.Text.Json;
using System.Threading;
using System.Threading.Tasks;
using AvaloniaApplication1.Config.Exceptions;

namespace AvaloniaApplication1.Config.Stores;

public class JsonConfigStore(ConfigEnvironment configEnvironment) : IConfigStore
{
    private static readonly JsonSerializerOptions JsonSerializerOptions = new() { WriteIndented = true };

    public async Task SaveAsync(AppConfig config, CancellationToken cancellationToken = default)
    {
        var json = JsonSerializer.Serialize(config, JsonSerializerOptions);
        Directory.CreateDirectory(configEnvironment.DirectoryPath);
        await File.WriteAllTextAsync(configEnvironment.FilePath, json, cancellationToken);
    }

    public async Task<AppConfig> LoadAsync(CancellationToken cancellationToken = default)
    {
        var json = await File.ReadAllTextAsync(configEnvironment.FilePath, cancellationToken);
        return JsonSerializer.Deserialize<AppConfig>(json) ?? throw new InvalidConfigException("Config is null");
    }
}