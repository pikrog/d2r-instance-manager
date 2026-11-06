using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.GlobalSettings;

public interface IGlobalSettingsPathValidator
{
    GameExecutablePathIssue? Validate(string path);
}