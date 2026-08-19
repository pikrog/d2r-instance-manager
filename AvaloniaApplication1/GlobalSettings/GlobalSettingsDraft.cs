namespace AvaloniaApplication1.GlobalSettings;

public record GlobalSettingsDraft(
    string GameExecutablePath, 
    bool CenterMouseCursorInShownWindow,
    bool FallbackToPrimaryDisplayIfInvalid,
    bool CloseInstancesOnAppExit
);