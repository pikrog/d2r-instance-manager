namespace AvaloniaApplication1.Display;

public abstract record DisplayOption
{
    public sealed record Primary : DisplayOption
    {
        public static readonly Primary Instance = new();
    }

    public sealed record Specific(int? Index, string Id, string Description, int Width, int Height, bool IsConnected) : DisplayOption
    {
        public string Label => $"{(Index is not null ? $"{Index} - " : "")}{Description} ({Width}x{Height}){(IsConnected ? "" : " [disconnected]")}";
    }

    public static readonly DisplayOption Default = Primary.Instance;
};