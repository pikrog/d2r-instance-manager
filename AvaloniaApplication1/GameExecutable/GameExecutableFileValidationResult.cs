namespace D2RInstanceManager.GameExecutable;

public readonly record struct GameExecutableFileValidationResult(
    GameExecutableFileValidationResultCode Code,
    FileMetadata? FileMetadata = null);