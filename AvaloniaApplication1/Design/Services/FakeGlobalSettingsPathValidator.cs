using D2RInstanceManager.GlobalSettings;
using D2RInstanceManager.GlobalSettings.Issues;

namespace D2RInstanceManager.Design.Services;

public class FakeGlobalSettingsPathValidator : IGlobalSettingsPathValidator
{
    public GameExecutablePathIssue Validate(string path) => new MissingExecutablePath();
}