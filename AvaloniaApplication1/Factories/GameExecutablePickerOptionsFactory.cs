using Avalonia.Platform.Storage;
using AvaloniaApplication1.Constants;

namespace AvaloniaApplication1.Factories;

public static class GameExecutablePickerOptionsFactory
{
    private const string FilePickerExecutableTypeName = "Executable files";
    private const string FilePickerExecutableTypeExtension = "*.exe";

    public static FilePickerOpenOptions Create() => new()
    {
        SuggestedFileName = GameConstants.ExecutableName,
        FileTypeFilter =
        [
            new FilePickerFileType(FilePickerExecutableTypeName)
            {
                Patterns = [FilePickerExecutableTypeExtension]
            }
        ]
    };
}