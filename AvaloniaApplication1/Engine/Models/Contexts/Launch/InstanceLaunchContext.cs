namespace AvaloniaApplication1.Engine.Models.Contexts.Launch;

public record InstanceLaunchContext
(
    string ExecutablePath,
    AuthenticationContext AuthenticationContext,
    int DisplayId,
    bool IsNoSound,
    bool IsWindowedMode,
    bool FallbackToPrimaryDisplayIfInvalid
);