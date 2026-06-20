namespace AvaloniaApplication1.Models;

public record GlobalSettingsDraft(
    string GameExecutablePath, 
    bool CenterMouseCursorInRecalledWindow,
    bool FallbackToPrimaryDisplayIfInvalid,
    bool CloseInstancesOnAppExit
);