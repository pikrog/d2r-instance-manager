namespace AvaloniaApplication1.Models;

public abstract record DisplayOption
{
    public sealed record Primary : DisplayOption;

    public sealed record Specific(int Index, string Id, string Description, int Width, int Height) : DisplayOption
    {
        public string Label => $"{Index} - {Description} ({Width}x{Height})";
    }
};