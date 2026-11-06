namespace AvaloniaApplication1.GameExecutable;

public readonly record struct GameExecutableFileValidationResult(
    GameExecutableFileValidationResultCode Code,
    FileMetadata? FileMetadata = null);