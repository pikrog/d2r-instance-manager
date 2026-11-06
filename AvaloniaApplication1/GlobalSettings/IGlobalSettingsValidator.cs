using System.Collections.Generic;
using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.GlobalSettings;

public interface IGlobalSettingsValidator
{
    IReadOnlyList<GlobalSettingsIssue> Validate();
}