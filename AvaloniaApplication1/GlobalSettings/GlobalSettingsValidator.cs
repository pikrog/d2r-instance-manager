using System.Collections.Generic;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.GlobalSettings;

public class GlobalSettingsValidator(ConfigService configService, IGlobalSettingsPathValidator pathValidator) 
    : IGlobalSettingsValidator
{
    public IReadOnlyList<GlobalSettingsIssue> Validate()
    {
        var issues = new List<GlobalSettingsIssue>();

        var settings = configService.Config.GetGlobalSettings();
        
        var path = settings.GameExecutablePath;
        var executableValidationIssue = pathValidator.Validate(path);
        
        if (executableValidationIssue is not null)
            issues.Add(executableValidationIssue);
        
        return issues;
    }
}