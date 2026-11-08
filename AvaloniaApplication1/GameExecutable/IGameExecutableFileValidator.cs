namespace D2RInstanceManager.GameExecutable;

public interface IGameExecutableFileValidator
{
    GameExecutableFileValidationResult Validate(string path);
}