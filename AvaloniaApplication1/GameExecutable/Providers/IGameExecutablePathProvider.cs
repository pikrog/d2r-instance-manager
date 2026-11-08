namespace D2RInstanceManager.GameExecutable.Providers;

public interface IGameExecutablePathProvider
{
    string? TryGet();
}