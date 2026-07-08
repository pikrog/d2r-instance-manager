using Avalonia.Platform.Storage;

namespace AvaloniaApplication1.GameExecutable;

public static class GameExecutablePickerOptionsFactory
{
    private const string FilePickerExecutableTypeName = "Executable files";
    private const string FilePickerExecutableTypeExtension = "*.exe";

    public static FilePickerOpenOptions Create() => new()
    {
        SuggestedFileName = GameExecutableConstants.ExecutableName,
        FileTypeFilter =
        [
            new FilePickerFileType(FilePickerExecutableTypeName)
            {
                Patterns = [FilePickerExecutableTypeExtension]
            }
        ]
    };
}