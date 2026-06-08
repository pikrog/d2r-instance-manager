namespace AvaloniaApplication1.Engine.Models.Contexts.Arguments;

public record ArgumentsContext
{
    public AuthenticationArgumentsContext AuthenticationArgumentsContext { get; init; } = new OfflineArgumentsContext();
    public bool IsNoSound { get; init; }
    public bool IsWindowedMode { get; init; }
}