using System.Collections.Generic;
using D2RInstanceManager.GlobalSettings.Issues;

namespace D2RInstanceManager.GlobalSettings;

public interface IGlobalSettingsValidator
{
    IReadOnlyList<GlobalSettingsIssue> Validate();
}