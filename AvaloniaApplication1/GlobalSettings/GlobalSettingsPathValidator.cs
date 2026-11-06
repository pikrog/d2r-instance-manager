using System;
using AvaloniaApplication1.GameExecutable;
using AvaloniaApplication1.GlobalSettings.Issues;

namespace AvaloniaApplication1.GlobalSettings;

public class GlobalSettingsPathValidator(IGameExecutableFileValidator validator) : IGlobalSettingsPathValidator
{
    public GameExecutablePathIssue? Validate(string path)
    {
        var executableValidationResult = validator.Validate(path);
        return executableValidationResult.Code switch
        {
            GameExecutableFileValidationResultCode.Ok => null,
            GameExecutableFileValidationResultCode.MissingPath => new MissingExecutablePath(),
            GameExecutableFileValidationResultCode.FileNotFound => new ExecutableFileNotFound(path),
            GameExecutableFileValidationResultCode.InvalidExecutableFormat => new InvalidExecutableFileFormat(path),
            GameExecutableFileValidationResultCode.UnrecognizedExecutable => new UnrecognizedExecutable(path),
            _ => throw new InvalidOperationException($"Unexpected executable validation result code: {executableValidationResult.Code}")
        };
    }
}