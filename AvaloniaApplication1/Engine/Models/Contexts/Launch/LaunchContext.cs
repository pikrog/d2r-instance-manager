namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record LaunchContext
(
    string ExecutablePath,
    AuthenticationContext AuthenticationContext,
    int DisplayId,
    bool IsNoSound,
    bool IsWindowedMode
);