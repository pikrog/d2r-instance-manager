namespace AvaloniaApplication1.GlobalSettings;

public record GlobalSettingsDraft(
    string GameExecutablePath, 
    bool CenterMouseCursorInRecalledWindow,
    bool FallbackToPrimaryDisplayIfInvalid,
    bool CloseInstancesOnAppExit
);