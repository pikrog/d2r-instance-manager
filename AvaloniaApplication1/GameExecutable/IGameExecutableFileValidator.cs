namespace AvaloniaApplication1.GameExecutable;

public interface IGameExecutableFileValidator
{
    GameExecutableFileValidationResult Validate(string path);
}