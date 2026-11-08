using D2RInstanceManager.GlobalSettings.Issues;

namespace D2RInstanceManager.GlobalSettings;

public interface IGlobalSettingsPathValidator
{
    GameExecutablePathIssue? Validate(string path);
}