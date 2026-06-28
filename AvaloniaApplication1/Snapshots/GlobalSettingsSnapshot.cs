namespace AvaloniaApplication1.Snapshots;

using Models;

public record GlobalSettingsSnapshot
(
    string GameExecutablePath = "",
    int UnlockMultiboxMaxRetries = 10,
    int UnlockMultiboxRetryDelayMs = 500,
    int GracefulInstanceCloseTimeoutMs = 250,
    int GracefulInstanceCloseRetries = 20,
    int ForcefulInstanceCloseTimeoutMs = 3000,
    bool CenterMouseCursorInRecalledWindow = true, // per instance config? + minimize to tray if another window is recalled
    bool FallbackToPrimaryDisplayIfInvalid = true,
    bool CloseInstancesOnAppExit = true, // used only by GameInstanceService
    CredentialsVector CredentialsVector = CredentialsVector.OsiTokenRegistry
    // todo: option: osi: auto authentication / browser authentication
);