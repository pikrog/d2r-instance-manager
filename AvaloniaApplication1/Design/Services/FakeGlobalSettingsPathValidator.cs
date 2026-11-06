using AvaloniaApplication1.GlobalSettings;
using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.Design.Services;

public class FakeGlobalSettingsPathValidator : IGlobalSettingsPathValidator
{
    public GameExecutablePathIssue Validate(string path) => new MissingExecutablePath();
}