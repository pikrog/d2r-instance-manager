namespace AvaloniaApplication1.Snapshots;

public record GlobalSettingsSnapshot
(
    string GameExecutablePath = "",
    int CloseEventMaxRetries = 10,
    int CloseEventRetryDelayMs = 1000,
    int GracefulInstanceCloseTimeoutMs = 3000,
    int ForcefulInstanceCloseTimeoutMs = 3000,
    bool CenterMouseCursorInRecalledWindow = true, // per instance config? + minimize to tray if another window is recalled
    bool FallbackToPrimaryDisplayIfInvalid = true,
    bool CloseInstancesOnAppExit = true
);