using System;
using System.Collections.Generic;
using AvaloniaApplication1.Config;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.GlobalSettings;

public class GlobalSettingsValidator(ConfigService configService)
{
    public IReadOnlyList<GlobalSettingsIssue> Validate()
    {
        var issues = new List<GlobalSettingsIssue>();

        var settings = configService.Config.GetGlobalSettings();
        
        var path = settings.GameExecutablePath;
        var executableValidationResult = GameExecutableFileValidator.Validate(path);
        GlobalSettingsIssue? executableValidationIssue = executableValidationResult switch
        {
            GameExecutableFileValidator.Result.Ok => null,
            GameExecutableFileValidator.Result.MissingPath => new MissingExecutablePath(),
            GameExecutableFileValidator.Result.FileNotFound => new ExecutableFileNotFound(path),
            GameExecutableFileValidator.Result.InvalidExecutableFormat => new InvalidExecutableFileFormat(path),
            GameExecutableFileValidator.Result.UnrecognizedExecutable => new UnrecognizedExecutable(path),
            _ => throw new InvalidOperationException($"Unexpected executable validation result: {executableValidationResult}")
        };
        
        if (executableValidationIssue is not null)
            issues.Add(executableValidationIssue);
        
        return issues;
    }
}