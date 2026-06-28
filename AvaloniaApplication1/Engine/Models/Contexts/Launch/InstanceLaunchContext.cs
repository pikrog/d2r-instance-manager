namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record InstanceLaunchContext
(
    string ExecutablePath,
    AuthenticationContext AuthenticationContext,
    string DisplayId,
    bool IsNoSound,
    bool IsWindowedMode
);