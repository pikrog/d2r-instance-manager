namespace D2RInstanceManager.GlobalSettings;

public record GlobalSettingsDraft(
    string GameExecutablePath, 
    bool CenterMouseCursorInShownWindow,
    bool FallbackToPrimaryDisplayIfInvalid,
    bool CloseInstancesOnAppExit
);