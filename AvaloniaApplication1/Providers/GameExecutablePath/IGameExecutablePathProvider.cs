namespace AvaloniaApplication1.Providers.GameExecutablePath;

public interface IGameExecutablePathProvider
{
    string? TryGet();
}